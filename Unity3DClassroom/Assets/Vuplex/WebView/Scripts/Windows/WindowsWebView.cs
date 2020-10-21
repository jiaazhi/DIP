/**
* Copyright (c) 2020 Vuplex Inc. All rights reserved.
*
* Licensed under the Vuplex Commercial Software Library License, you may
* not use this file except in compliance with the License. You may obtain
* a copy of the License at
*
*     https://vuplex.com/commercial-library-license
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

#if NET_4_6
using System.Threading.Tasks;
#endif // NET_4_6

namespace Vuplex.WebView {
    /// <summary>
    /// The Windows `IWebView` implementation.
    /// </summary>
    public class WindowsWebView : StandaloneWebView, IWebView {

        public static WindowsWebView Instantiate() {

            return (WindowsWebView) new GameObject().AddComponent<WindowsWebView>();
        }

        public override void CaptureScreenshot(Action<byte[]> callback) {
            try {
                var temporaryFilePath = Path.Combine(Application.temporaryCachePath, Guid.NewGuid().ToString() + ".png");
                _pendingScreenshotCallbacks[temporaryFilePath] = callback;
                WebView_captureScreenshot(_nativeWebViewPtr, temporaryFilePath);
            } catch (Exception e) {
                Debug.LogError("An exception occurred while capturing the screenshot: " + e);
                callback(new byte[0]);
            }
        }

        public static new void CreateTexture(float width, float height, Action<Texture2D> callback) {

            var graphicsApiIsValid = ValidateGraphicsApi();
            if (graphicsApiIsValid) {
                BaseWebView.CreateTexture(width, height, callback);
            } else {
                callback(null);
            }
        }

        public override void Dispose() {

            // Cancel the render if it has been scheduled via GL.IssuePluginEvent().
            WebView_removePointer(_nativeWebViewPtr);
            base.Dispose();
        }

        public static void EnableDebugLogging() {

            // The generic `GetFunctionPointerForDelegate<T>` is unavailable in .NET 2.0.
            var logInfo = Marshal.GetFunctionPointerForDelegate((LogFunction)_logInfo);
            var logWarning = Marshal.GetFunctionPointerForDelegate((LogFunction)_logWarning);
            var logError = Marshal.GetFunctionPointerForDelegate((LogFunction)_logError);
            WebView_setLogFunctions(logInfo, logWarning, logError);
        }

        public override void GetRawTextureData(Action<byte[]> callback) {

            _invokeDataOperation(callback, WebView_getRawTextureData);
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        public void HandleScreenshotReady(string temporaryFilePath) {

            var bytes = new byte[0];
            var callback = _pendingScreenshotCallbacks[temporaryFilePath];
            _pendingScreenshotCallbacks.Remove(temporaryFilePath);
            try {
                bytes = File.ReadAllBytes(temporaryFilePath);
                File.Delete(temporaryFilePath);
            } catch (Exception e) {
                Debug.LogError("An exception occurred while getting the data for the CaptureScreenshot callback: " + e);
            }
            callback(bytes);
        }

        public static bool ValidateGraphicsApi() {

            var isValid = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11;
            if (!isValid) {
                Debug.LogError("Unsupported graphics API: 3D WebView for Windows requires Direct3D11. Please go to Player Settings and set \"Graphics APIs for Windows\" to Direct3D11.");
            }
            return isValid;
        }

        delegate void DataResultCallback(string gameObjectName, string resultCallbackId, IntPtr imageBytes, int imageBytesLength);
        delegate void LogFunction(string message);
        Dictionary<string, Action<byte[]>> _pendingDataResultCallbacks = new Dictionary<string, Action<byte[]>>();
        Dictionary<string, Action<byte[]>> _pendingScreenshotCallbacks = new Dictionary<string, Action<byte[]>>();

        [AOT.MonoPInvokeCallback(typeof(DataResultCallback))]
        static void _dataResultCallback(string gameObjectName, string resultCallbackId, IntPtr unmanagedBytes, int bytesLength) {

            // Load the results into a managed array.
            var managedBytes = new byte[bytesLength];
            Marshal.Copy(unmanagedBytes, managedBytes, 0, bytesLength);

            Dispatcher.RunOnMainThread(() => {
                var gameObj = GameObject.Find(gameObjectName);
                if (gameObj == null) {
                    Debug.LogErrorFormat("Unable to process the data result, because there is no GameObject named '{0}'", gameObjectName);
                    return;
                }
                var webView = gameObj.GetComponent<WindowsWebView>();
                webView._handleDataResult(resultCallbackId, managedBytes);
            });
        }

        void _handleDataResult(string resultCallbackId, byte[] bytes) {

            var callback = _pendingDataResultCallbacks[resultCallbackId];
            _pendingDataResultCallbacks.Remove(resultCallbackId);
            callback(bytes);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void _initializePlugin() {

            // The generic `GetFunctionPointerForDelegate<T>` is unavailable in .NET 2.0.
            var dataResultCallback = Marshal.GetFunctionPointerForDelegate((DataResultCallback)_dataResultCallback);
            WebView_setDataResultCallback(dataResultCallback);
        }

        void _invokeDataOperation(Action<byte[]> callback, Action<IntPtr, string> nativeDataMethod) {
            try {
                if (_isDisposed) {
                    if (callback != null) {
                        callback(new byte[0]);
                    }
                } else {
                    string resultCallbackId = null;
                    if (callback != null) {
                        resultCallbackId = Guid.NewGuid().ToString();
                        _pendingDataResultCallbacks[resultCallbackId] = callback;
                    }
                    nativeDataMethod(_nativeWebViewPtr, resultCallbackId);
                }
            } catch (Exception e) {
                Debug.LogError("An exception occurred in while getting the webview data: " + e);
                callback(new byte[0]);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(LogFunction))]
        static void _logInfo(string message) {

            Debug.Log(message);
        }

        [AOT.MonoPInvokeCallback(typeof(LogFunction))]
        static void _logWarning(string message) {

            Debug.LogWarning(message);
        }

        [AOT.MonoPInvokeCallback(typeof(LogFunction))]
        static void _logError(string message) {

            Debug.LogError(message);
        }

        IEnumerator _renderPluginOncePerFrame() {
            while (true) {
                // Wait until all frame rendering is done
                yield return new WaitForEndOfFrame();

                if (_nativeWebViewPtr != IntPtr.Zero && !_isDisposed) {
                    int pointerId = WebView_depositPointer(_nativeWebViewPtr);
                    GL.IssuePluginEvent(WebView_getRenderFunction(), pointerId);
                }
            }
        }

        IEnumerator Start() {

            yield return StartCoroutine(_renderPluginOncePerFrame());
        }

        [DllImport(_dllName)]
        private static extern void WebView_captureScreenshot(IntPtr webViewPtr, string filePath);

        [DllImport(_dllName)]
        static extern int WebView_depositPointer(IntPtr pointer);

        [DllImport(_dllName)]
        private static extern void WebView_getRawTextureData(IntPtr webViewPtr, string resultCallbackId);

        [DllImport(_dllName)]
        static extern IntPtr WebView_getRenderFunction();

        [DllImport(_dllName)]
        static extern void WebView_removePointer(IntPtr pointer);

        [DllImport(_dllName)]
        static extern int WebView_setDataResultCallback(IntPtr callback);

        [DllImport(_dllName)]
        static extern int WebView_setLogFunctions(
            IntPtr logInfoFunction,
            IntPtr logWarningFunction,
            IntPtr logErrorFunction
        );
    }
}
#endif
