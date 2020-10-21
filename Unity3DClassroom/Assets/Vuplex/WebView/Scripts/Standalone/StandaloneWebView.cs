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
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vuplex.WebView {
    /// <summary>
    /// The base `IWebView` implementation used by 3D WebView for Windows and macOS.
    /// This class also includes extra methods for Standalone-specific functionality.
    /// </summary>
    public abstract class StandaloneWebView : BaseWebView {

        public override void Init(Texture2D viewportTexture, float width, float height, Texture2D videoTexture) {

            base.Init(viewportTexture, width, height, videoTexture);
            _nativeWebViewPtr = WebView_new(gameObject.name, _nativeWidth, _nativeHeight);
            if (_nativeWebViewPtr == IntPtr.Zero) {
                throw new WebViewUnavailableException("Failed to instantiate a new webview. This could indicate that you're using an expired trial version of 3D WebView.");
            }
        }

        public override void CanGoBack(Action<bool> callback) {

            _pendingCanGoBackCallbacks.Add(callback);
            WebView_canGoBack(_nativeWebViewPtr);
        }

        public override void CanGoForward(Action<bool> callback) {

            _pendingCanGoForwardCallbacks.Add(callback);
            WebView_canGoForward(_nativeWebViewPtr);
        }

        public static void ClearAllData() {

            var pluginIsInitialized = WebView_pluginIsInitialized();
            if (pluginIsInitialized) {
                _throwAlreadyInitializedException("clear the browser data", "ClearAllData");
            }
            var cachePath = _getCachePath();
            if (Directory.Exists(cachePath)) {
                Directory.Delete(cachePath, true);
            }
        }

        /// <summary>
        /// Enables remote debugging with Chrome DevTools on the given port.
        /// Note that this method can only be called prior to
        /// creating any webviews.
        /// </summary>
        /// <remarks>
        /// For example, if you provide 8080 as the `portNumber`, you can go to
        /// http://localhost:8080 in Chrome to see a list of webviews to inspect.
        /// </remarks>
        /// <param name="portNumber">Port number in the range 1024 - 65535.</param>
        public static void EnableRemoteDebugging(int portNumber) {

            if (!(1024 <= portNumber && portNumber <= 65535)) {
                throw new ArgumentException(string.Format("The given port number ({0}) is not in the range from 1024 to 65535.", portNumber));
            }
            var success = WebView_enableRemoteDebugging(portNumber);
            if (!success) {
                _throwAlreadyInitializedException("enable remote debugging", "EnableRemoteDebugging");
            }
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        public void HandleCanGoBackResult(string message) {

            var result = Boolean.Parse(message);
            var callbacks = new List<Action<bool>>(_pendingCanGoBackCallbacks);
            _pendingCanGoBackCallbacks.Clear();
            foreach (var callback in callbacks) {
                try {
                    callback(result);
                } catch (Exception e) {
                    Debug.LogError("An exception occurred while calling the callback for CanGoBack: " + e);
                }
            }
        }

        /// <summary>
        /// The native plugin invokes this method.
        /// </summary>
        public void HandleCanGoForwardResult(string message) {

            var result = Boolean.Parse(message);
            var callBacks = new List<Action<bool>>(_pendingCanGoForwardCallbacks);
            _pendingCanGoForwardCallbacks.Clear();
            foreach (var callBack in callBacks) {
                try {
                    callBack(result);
                } catch (Exception e) {
                    Debug.LogError("An exception occurred while calling the callForward for CanGoForward: " + e);
                }
            }
        }

        /// <summary>
        /// Moves the pointer to the given point in the webpage.
        /// This can be used, for example, to trigger hover effects in the page.
        /// </summary>
        /// <param name="point">
        /// The x and y components of the point are values
        /// between 0 and 1 that are normalized to the width and height, respectively. For example,
        /// `point.x = x in Unity units / width in Unity units`.
        /// Like in the browser, the origin is in the upper-left corner,
        /// the positive direction of the y-axis is down, and the positive
        /// direction of the x-axis is right.
        /// </param>
        public void MovePointer(Vector2 point) {

            int nativeX = (int) (point.x * _nativeWidth);
            int nativeY = (int) (point.y * _nativeHeight);
            WebView_movePointer(_nativeWebViewPtr, nativeX, nativeY);
        }

        /// <summary>
        /// Dispatches a mouse down click event.
        /// This can be used, for example, to implement drag-and-drop.
        /// </summary>
        /// <param name="point">
        /// The x and y components of the point are values
        /// between 0 and 1 that are normalized to the width and height, respectively. For example,
        /// `point.x = x in Unity units / width in Unity units`.
        /// Like in the browser, the origin is in the upper-left corner,
        /// the positive direction of the y-axis is down, and the positive
        /// direction of the x-axis is right.
        /// </param>
        /// <seealso cref="Click"/>
        public void PointerDown(Vector2 point) {

            int nativeX = (int) (point.x * _nativeWidth);
            int nativeY = (int) (point.y * _nativeHeight);
            WebView_pointerDown(_nativeWebViewPtr, nativeX, nativeY);
        }

        /// <summary>
        /// Dispatches a mouse up click event.
        /// This can be used, for example, to implement drag-and-drop.
        /// </summary>
        /// <param name="point">
        /// The x and y components of the point are values
        /// between 0 and 1 that are normalized to the width and height, respectively. For example,
        /// `point.x = x in Unity units / width in Unity units`.
        /// Like in the browser, the origin is in the upper-left corner,
        /// the positive direction of the y-axis is down, and the positive
        /// direction of the x-axis is right.
        /// </param>
        /// <seealso cref="Click"/>
        public void PointerUp(Vector2 point) {

            int nativeX = (int) (point.x * _nativeWidth);
            int nativeY = (int) (point.y * _nativeHeight);
            WebView_pointerUp(_nativeWebViewPtr, nativeX, nativeY);
        }

        public override void Resize(float width, float height) {

            // There's currently an issue in CEF where decreasing the width
            // without changing the height or vice versa can cause graphics
            // glitches. As a workaround, if one of the dimensions is decreased
            // and the other is unchanged, an initial temporary resize is done
            // to temporarily change the unchanged dimension so that both dimensions
            // are different for the second resize.
            if (_isDisposed || !_viewportTexture) {
                return;
            }
            var widthDecreasedButHeightIsTheSame = width < _width && height == _height;
            var heightDecreasedButWidthIsTheSame = height < _height && width == _width;

            if (widthDecreasedButHeightIsTheSame || heightDecreasedButWidthIsTheSame) {
                var tempWidthPixels = (int)(width * _numberOfPixelsPerUnityUnit);
                if (width == _width) {
                    tempWidthPixels++;
                }
                var tempHeightPixels = (int)(height * _numberOfPixelsPerUnityUnit);
                if (height == _height) {
                    tempHeightPixels++;
                }
                WebView_resize(_nativeWebViewPtr, tempWidthPixels, tempHeightPixels);
            }
            base.Resize(width, height);
        }

        /// <summary>
        /// By default, web pages cannot access the device's
        /// camera or microphone via JavaScript.
        /// Invoking `SetAudioAndVideoCaptureEnabled(true)` allows
        /// **all web pages** to access the camera and microphone.
        /// </summary>
        /// <remarks>
        /// This is useful, for example, to enable WebRTC support.
        /// Note that this method can only be called prior to creating any webviews.
        /// </remarks>
        public static void SetAudioAndVideoCaptureEnabled(bool enabled) {

            var success = WebView_setAudioAndVideoCaptureEnabled(enabled);
            if (!success) {
                _throwAlreadyInitializedException("enable audio and video capture", "SetAudioAndVideoCaptureEnabled");
            }
        }

        /// <summary>
        /// Sets additional command line arguments that will be passed
        /// to Chromium.
        /// <summary>
        /// <remarks>
        /// [Unofficial list of command line arguments](https://peter.sh/experiments/chromium-command-line-switches/).
        /// Note that this method can only be called prior to creating any webviews.
        /// </remarks>
        /// <example>
        /// StandaloneWebView.SetCommandLineArguments("--ignore-certificate-errors --disable-web-security");
        /// </example>
        public static void SetCommandLineArguments(string args) {

            var success = WebView_setCommandLineArguments(args);
            if (!success) {
                _throwAlreadyInitializedException("set command line arguments", "SetCommandLineArguments");
            }
        }

        public static void SetStorageEnabled(bool enabled) {

            var cachePath = enabled ? _getCachePath() : "";
            var success = WebView_setCachePath(cachePath);
            if (!success) {
                _throwAlreadyInitializedException("enable or disable storage", "SetStorageEnabled");
            }
        }

        /// <summary>
        /// Change the zoom level to the specified value. Specify 0.0 to reset the zoom level.
        /// </summary>
        public void SetZoomLevel(float zoomLevel) {

            WebView_setZoomLevel(_nativeWebViewPtr, zoomLevel);
        }

        public static void TerminatePlugin() {

            if (!_pluginTerminated) {
                _pluginTerminated = true;
                WebView_terminatePlugin();
            }
        }

        List<Action<bool>> _pendingCanGoBackCallbacks = new List<Action<bool>>();
        List<Action<bool>> _pendingCanGoForwardCallbacks = new List<Action<bool>>();
        delegate void UnitySendMessageFunction(string gameObjectName, string methodName, string message);

        protected static string _getCachePath() {

            // Only `Path.Combine(string, string)` is available in .NET 2.0.
            return Path.Combine(Application.persistentDataPath, Path.Combine("Vuplex.WebView", "chromium-cache"));
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void _initializePlugin() {

            // The generic `GetFunctionPointerForDelegate<T>` is unavailable in .NET 2.0.
            var sendMessageFunction = Marshal.GetFunctionPointerForDelegate((UnitySendMessageFunction)_unitySendMessage);
            WebView_setSendMessageFunction(sendMessageFunction);
            SetStorageEnabled(true); // cache, cookies, and storage are enabled by default
        }

        void MessageHandler_JavaScriptResultReceived(object sender, EventArgs<StringWithIdBridgeMessage> e) {

            var resultCallbackId = e.Value.id;
            var result = e.Value.value;
            var callback = _pendingJavaScriptResultCallbacks[resultCallbackId];
            _pendingJavaScriptResultCallbacks.Remove(resultCallbackId);
            callback(result);
        }

        static void _throwAlreadyInitializedException(string action, string methodName) {

            var message = String.Format("Unable to {0}, because a webview has already been created. On Windows and macOS, {1}() can only be called prior to creating any webviews.", action, methodName);
            throw new InvalidOperationException(message);
        }

        [AOT.MonoPInvokeCallback(typeof(UnitySendMessageFunction))]
        static void _unitySendMessage(string gameObjectName, string methodName, string message) {

            Dispatcher.RunOnMainThread(() => {
                var gameObj = GameObject.Find(gameObjectName);
                if (gameObj == null) {
                    Debug.LogErrorFormat("Unable to send the message, because there is no GameObject named '{0}'", gameObjectName);
                    return;
                }
                gameObj.SendMessage(methodName, message);
            });
        }

        [DllImport(_dllName)]
        static extern void WebView_canGoBack(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern void WebView_canGoForward(IntPtr webViewPtr);

        [DllImport(_dllName)]
        static extern bool WebView_enableRemoteDebugging(int portNumber);

        [DllImport(_dllName)]
        static extern IntPtr WebView_new(string gameObjectName, int width, int height);

        [DllImport (_dllName)]
        private static extern void WebView_movePointer(IntPtr webViewPtr, int x, int y);

        [DllImport(_dllName)]
        static extern bool WebView_pluginIsInitialized();

        [DllImport (_dllName)]
        private static extern void WebView_pointerDown(IntPtr webViewPtr, int x, int y);

        [DllImport (_dllName)]
        private static extern void WebView_pointerUp(IntPtr webViewPtr, int x, int y);

        [DllImport(_dllName)]
        static extern bool WebView_setAudioAndVideoCaptureEnabled(bool enabled);

        [DllImport(_dllName)]
        static extern bool WebView_setCachePath(string cachePath);

        [DllImport(_dllName)]
        static extern bool WebView_setCommandLineArguments(string args);

        [DllImport(_dllName)]
        static extern int WebView_setSendMessageFunction(IntPtr sendMessageFunction);

        [DllImport(_dllName)]
        static extern void WebView_setZoomLevel(IntPtr webViewPtr, float zoomLevel);

        [DllImport(_dllName)]
        static extern void WebView_terminatePlugin();
    }
}

#endif // UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
