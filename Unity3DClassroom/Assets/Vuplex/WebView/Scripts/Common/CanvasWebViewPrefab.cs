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
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Vuplex.WebView {
    /// <summary>
    /// A prefab that makes it easy to create and interact with an `IWebView` in a Canvas.
    /// Important notes:
    /// - The Canvas's Render Mode must be set to either "Screen Space - Camera" or
    /// "World Space"; "Screen Space - Overlay" currently is not supported.
    /// - A Physics Raycaster must be attached to the camera to enable click events.
    /// </summary>
    /// <remarks>
    /// `CanvasWebViewPrefab` takes care of creating and initializing an `IWebView`, displaying its texture,
    /// and handling click and scroll interactions from the user. So, all you need to do is load some web
    /// content from a URL or HTML string, and then the user can view and interact with it.
    ///
    /// You can create a `CanvasWebViewPrefab` one of the following ways:
    /// - By dragging CanvasWebViewPrefab.prefab into a Canvas via the editor and setting its "Initial URL" property.
    /// - By instantiating an instance at runtime with `CanvasWebViewPrefab.Instantiate()` and then
    ///   waiting for its `Initialized` event, after which you can call methods on its `WebView` property.
    ///
    /// `CanvasWebViewPrefab` handles standard events from Unity's input system
    /// (like `IPointerDownHandler`), so it works with input modules that extend Unity's `BaseInputModule`,
    /// like Unity's Standalone Input Module and third-party modules.
    ///
    /// If your use case requires a high degree of customization, you can instead create an `IWebView`
    /// outside of the prefab with `Web.CreateWebView()`.
    /// </remarks>
    public class CanvasWebViewPrefab : MonoBehaviour {

        /// <summary>
        /// Indicates that the prefab finished initializing,
        /// so its `WebView` property is ready to use.
        /// </summary>
        public EventHandler Initialized;

        /// <summary>
        /// If you drag a CanvasWebViewPrefab.prefab into the scene via the editor,
        /// you can set this property in the editor to make it so that the instance
        /// automatically initializes itself with the given URL.
        /// </summary>
        [Label("Initial URL to Load (Optional)")]
        public string InitialUrl;

        /// <summary>
        /// If you drag a CanvasWebViewPrefab.prefab into the scene via the editor,
        /// you can set this property in the editor to determine the webview's
        /// resolution (in pixels per Unity unit).
        /// </summary>
        [Label("Initial Resolution (Pixels per Unity Unit)")]
        public int InitialResolution = 1;

        /// <summary>
        /// A reference to the prefab's `IWebView` instance, which
        /// is available after the `Initialized` event is raised.
        /// Before initialization is complete, this property is `null`.
        /// </summary>
        public IWebView WebView {
            get {
                return _webViewPrefab == null ? null : _webViewPrefab.WebView;
            }
        }

        /// <summary>
        /// Creates a new instance and initializes it asynchronously.
        /// </summary>
        /// <remarks>
        /// The `WebView` property is available after initialization completes,
        /// which is indicated by the `Initialized` event.
        /// </remarks>
        /// <example>
        /// Example:
        ///
        /// ```
        /// var canvas = GameObject.Find("Canvas");
        /// canvasWebViewPrefab = CanvasWebViewPrefab.Instantiate();
        /// canvasWebViewPrefab.transform.parent = canvas.transform;
        /// var rectTransform = canvasWebViewPrefab.transform as RectTransform;
        /// rectTransform.anchoredPosition3D = Vector3.zero;
        /// rectTransform.offsetMin = Vector2.zero;
        /// rectTransform.offsetMax = Vector2.zero;
        /// canvasWebViewPrefab.transform.localScale = Vector3.one;
        /// canvasWebViewPrefab.Initialized += (sender, e) => {
        ///     canvasWebViewPrefab.WebView.LoadUrl("https://vuplex.com");
        /// };
        /// ```
        /// </example>
        public static CanvasWebViewPrefab Instantiate() {

            return Instantiate(new WebViewOptions());
        }

        /// <summary>
        /// Like `Instantiate()`, except it also accepts an object
        /// of options flags that can be used to alter the generated webview's behavior.
        /// </summary>
        public static CanvasWebViewPrefab Instantiate(WebViewOptions options) {

            var prefabPrototype = (GameObject) Resources.Load("CanvasWebViewPrefab");
            var gameObject = (GameObject) Instantiate(prefabPrototype);
            var canvasWebViewPrefab = gameObject.GetComponent<CanvasWebViewPrefab>();
            canvasWebViewPrefab.Init(options);
            return canvasWebViewPrefab;
        }

        /// <summary>
        /// Asynchronously initializes the instance.
        /// </summary>
        /// <remarks>
        /// You only need to call this method if you place a CanvasWebViewPrefab.prefab in your
        /// scene via the Unity editor but don't set its "Initial URL" property.
        /// You don't need to call this method if you set the "Initial URL" property in
        /// the editor or if you instantiate the prefab programmatically at runtime using
        /// `Instantiate()`. In those cases, this method is called automatically for you.
        /// This method asynchronously initializes the `WebView` property, which is
        /// available for use after the `Initialized` event is raised.
        /// </remarks>
        public void Init() {

            Init(new WebViewOptions());
        }

        /// <summary>
        /// Like `Init()`, except it also accepts an object
        /// of options flags that can be used to alter the webview's behavior.
        /// </summary>
        public void Init(WebViewOptions options) {

            // The image is only used as a placeholder to show the bounds of the
            // CanvasWebViewPrefab in the editor, so disable it at runtime.
            GetComponent<Image>().enabled = false;

            // Instantiate it with an initial size of 1 x 1 to prevent
            // a huge initial size.
            _webViewPrefab = WebViewPrefab.Instantiate(1, 1, options);
            _webViewPrefab.Initialized += WebViewPrefab_Initialized;
            _webViewPrefab.transform.SetParent(transform, false);
            _webViewPrefab.transform.localPosition = new Vector3(0, _getRect().height / 2, 0);
            _webViewPrefab.transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        public WebViewPrefab _webViewPrefab;

        Rect _getRect() {

            return GetComponent<RectTransform>().rect;
        }

        void Awake() {

            if (!String.IsNullOrEmpty(InitialUrl)) {
                Init();
            }
        }

        void Update() {

            if (_webViewPrefab == null || _webViewPrefab.WebView == null) {
                return;
            }
            var rect = _getRect();
            var size = _webViewPrefab.WebView.Size;
            if (!(rect.width == size.x && rect.height == size.y)) {
                _webViewPrefab.Resize(rect.width, rect.height);
            }
            var expectedPosition = new Vector3(0, rect.height / 2, 0);
            if (_webViewPrefab.transform.localPosition != expectedPosition) {
                _webViewPrefab.transform.localPosition = expectedPosition;
            }
        }

        void WebViewPrefab_Initialized(object sender, EventArgs e) {

            var resolution = InitialResolution;
            if (InitialResolution <= 0) {
                Debug.LogWarningFormat("Invalid value for CanvasWebViewPrefab.InitialResolution: {0}. The resolution will instead default to 1.", InitialResolution);
                resolution = 1;
            }
            _webViewPrefab.WebView.SetResolution(resolution);

            // Resize to the actual desired size.
            var rect = _getRect();
            _webViewPrefab.Resize(rect.width, rect.height);

            var handler = Initialized;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }

            if (!String.IsNullOrEmpty(InitialUrl)) {
                var url = InitialUrl.Trim();
                if (!url.Contains(":")) {
                    url = "http://" + url;
                }
                _webViewPrefab.WebView.LoadUrl(url);
            }
        }
    }
}
