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
#if (UNITY_STANDALONE_OSX && !UNITY_EDITOR) || UNITY_EDITOR_OSX
using System;
using System.Runtime.InteropServices;
using UnityEngine;

#if NET_4_6
using System.Threading.Tasks;
#endif // NET_4_6

namespace Vuplex.WebView {

    /// <summary>
    /// The macOS `IWebView` implementation.
    /// </summary>
    public class MacWebView : StandaloneWebView, IWebView {

        public static MacWebView Instantiate() {

            return (MacWebView) new GameObject().AddComponent<MacWebView>();
        }

        public override void CaptureScreenshot(Action<byte[]> callback) {

            IntPtr unmanagedBytes = IntPtr.Zero;
            int unmanagedBytesLength = 0;
            WebView_captureScreenshot(_nativeWebViewPtr, ref unmanagedBytes, ref unmanagedBytesLength);

            // Load the results into a managed array.
            var managedBytes = new byte[unmanagedBytesLength];
            Marshal.Copy(unmanagedBytes, managedBytes, 0, unmanagedBytesLength);
            WebView_freeMemory(unmanagedBytes);
            callback(managedBytes);
        }

        public static new void CreateTexture(float width, float height, Action<Texture2D> callback) {

            var graphicsApiIsValid = ValidateGraphicsApi();
            if (graphicsApiIsValid) {
                BaseWebView.CreateTexture(width, height, callback);
            } else {
                callback(null);
            }
        }

        public static bool ValidateGraphicsApi() {

            var isValid = SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Metal;
            if (!isValid) {
                var message = "Unsupported graphics API: 3D WebView for macOS requires Metal.";
                if (Application.isEditor) {
                    message += " Please go to Player Settings and enable \"Metal Editor Support\".";
                }
                Debug.LogError(message);
            }
            return isValid;
        }

        [DllImport (_dllName)]
        private static extern void WebView_captureScreenshot(IntPtr webViewPtr, ref IntPtr bytes, ref int length);

        [DllImport (_dllName)]
        static extern void WebView_executeJavaScript(IntPtr webViewPtr, string javaScript, string resultCallbackId);
    }
}

#endif
