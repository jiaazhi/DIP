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
using UnityEngine;
using SimpleJSON;

namespace Vuplex.WebView.Demos {

    /// <summary>
    /// Sets up the CanvasWebViewDemo scene, which displays a `CanvasWebViewPrefab`
    /// in screen space inside a canvas.
    /// </summary>
    /// <remarks>
    /// This scene includes Unity's standalone input module, so
    /// you can click and scroll the webview using your touchscreen
    /// or mouse.
    ///
    /// You can also move the camera by holding down the control key on your
    /// keyboard and moving your mouse. When running on a device
    /// with a gyroscope, the gyroscope controls the camera rotation instead.
    ///
    /// `WebViewPrefab` handles standard Unity input events, so it works with
    /// a variety of third party input modules that extend Unity's `BaseInputModule`,
    /// like the input modules from the Google VR and Oculus VR SDKs.
    ///
    /// Here are some other examples that show how to use 3D WebView with popular SDKs:
    /// • Google VR (Cardboard and Daydream): https://github.com/vuplex/google-vr-webview-example
    /// • Oculus (Oculus Quest, Go, and Gear VR): https://github.com/vuplex/oculus-webview-example
    /// • AR Foundation : https://github.com/vuplex/ar-foundation-webview-example
    /// </remarks>
    class CanvasWebViewVuplexTest : MonoBehaviour {

        CanvasWebViewPrefab _canvasWebViewPrefab;
        HardwareKeyboardListener _hardwareKeyboardListener;

        void Start() {

            StandaloneWebView.EnableRemoteDebugging(8080);

            // The CanvasWebViewPrefab's `InitialUrl` property is set via the editor, so it
            // will automatically initialize itself with that URL.
            _canvasWebViewPrefab = GameObject.Find("CanvasWebViewPrefab").GetComponent<CanvasWebViewPrefab>();

            _canvasWebViewPrefab.Initialized += (sender, e) => {
                // Use the LoadProgressChanged event to determine when the page has loaded.
                _canvasWebViewPrefab.WebView.LoadProgressChanged += CanvasWebView_LoadProgressChanged;
                _canvasWebViewPrefab.WebView.MessageEmitted += CanvasWebView_MessageEmitted;
            };

            // Send keys from the hardware keyboard to the webview.
            _hardwareKeyboardListener = HardwareKeyboardListener.Instantiate();
            _hardwareKeyboardListener.InputReceived += (sender, eventArgs) => {
                _canvasWebViewPrefab.WebView.HandleKeyboardInput(eventArgs.Value);
            };
        }

        void CanvasWebView_LoadProgressChanged(object sender, ProgressChangedEventArgs eventArgs) {
            // Send a message after the page has loaded.
            if (eventArgs.Type == ProgressChangeType.Finished) {
                _canvasWebViewPrefab.WebView.PostMessage("Something changed!");
            }
        }

        void CanvasWebView_MessageEmitted(object sender, EventArgs<string> eventArgs) {
            var payload = JSON.Parse(eventArgs.Value);
            if (payload["type"].Value != "LOGIN_DATA") return;


            var login_data = payload["data"];
            var username = login_data["username"].Value;
            var roomname = login_data["roomname"].Value;

            Debug.Log("Username: " + username + ", Roomname: " + roomname);
            Debug.Log("You are logged in!");
        }
    }
}
