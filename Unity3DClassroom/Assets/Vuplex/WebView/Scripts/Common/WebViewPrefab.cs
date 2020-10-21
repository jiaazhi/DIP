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
using UnityEngine.EventSystems;

namespace Vuplex.WebView {
    /// <summary>
    /// A prefab that makes it easy to create and interact with an `IWebView` in world space.
    /// </summary>
    /// <remarks>
    /// `WebViewPrefab` takes care of creating and initializing an `IWebView`, displaying its texture,
    /// and handling click and scroll interactions from the user. So, all you need to do is load some web
    /// content from a URL or HTML string, and then the user can view and interact with it.
    ///
    /// You can create a `WebViewPrefab` one of the following ways:
    /// - By dragging WebViewPrefab.prefab into your scene via the editor and setting its "Initial URL" property.
    /// - By instantiating an instance at runtime with `WebViewPrefab.Instantiate()` and then
    ///   waiting for its `Initialized` event to be raised, after which you can call methods on its `WebView` property.
    ///
    /// `WebViewPrefab` handles standard events from Unity's input system
    /// (like `IPointerDownHandler`), so it works with input modules that extend Unity's `BaseInputModule`,
    /// like Unity's Standalone Input Module and third-party modules.
    ///
    /// If your use case requires a high degree of customization, you can instead create an `IWebView`
    /// outside of the prefab using `Web.CreateWebView()` and initialize it with a texture created
    /// with `Web.CreateMaterial()`.
    /// </remarks>
    public class WebViewPrefab : MonoBehaviour {

        /// <summary>
        /// Indicates that the prefab was clicked. Note that the prefab automatically
        /// calls the `IWebView.Click()` method for you.
        /// </summary>
        public event EventHandler Clicked;

        /// <summary>
        /// Indicates that the prefab finished initializing,
        /// so its `WebView` property is ready to use.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Clicking is enabled by default, but can be disabled by
        /// setting this property to `false`.
        /// </summary>
        public bool ClickingEnabled {
            get { return _clickingEnabled; }
            set { _clickingEnabled = value; }
        }

        /// <summary>
        /// In order to prevent dragging-to-scroll from triggering unwanted
        /// clicks, a click is discarded if the pointer drags beyond this
        /// threshold before a "pointer up" event occurs. The default threshold is 0.15.
        /// </summary>
        public float DragToScrollThreshold {
            get { return _dragToScrollThreshold; }
            set { _dragToScrollThreshold = value; }
        }

        /// <summary>
        /// If you drag a WebViewPrefab.prefab into the scene via the editor,
        /// you can set this property to make it so that the instance
        /// automatically initializes itself with the given URL.
        /// </summary>
        [Label("Initial URL to load (optional)")]
        public string InitialUrl;

        /// <summary>
        /// Scrolling is enabled by default, but can be disabled by
        /// setting this property to `false`.
        /// </summary>
        public bool ScrollingEnabled {
            get { return _scrollingEnabled; }
            set { _scrollingEnabled = value; }
        }

        /// <summary>
        /// Allows the scroll sensitivity to be adjusted.
        /// The default sensitivity is 0.001.
        /// </summary>
        public static float ScrollSensitivity {
            get { return _scrollSensitivity; }
            set { _scrollSensitivity = value; }
        }

        /// <summary>
        /// Controls whether the instance is visible or hidden.
        /// </summary>
        public bool Visible {
            // Use _getView and _getVideoLayer in case the instance isn't
            // initialized yet.
            get {
                return _getView().Visible || _getVideoLayer().Visible;
            }
            set {
                _getView().Visible = value;
                _getVideoLayer().Visible = value;
            }
        }

        /// <summary>
        /// A reference to the prefab's `IWebView` instance, which
        /// is available after the `Initialized` event is raised.
        /// Before initialization is complete, this property is `null`.
        /// </summary>
        public IWebView WebView { get { return _webView; }}

        /// <summary>
        /// Creates a new instance with the given
        /// dimensions in Unity units and initializes it asynchronously.
        /// </summary>
        /// <remarks>
        /// The `WebView` property is available after initialization completes,
        /// which is indicated by the `Initialized` event.
        /// </remarks>
        /// <example>
        /// Example:
        ///
        /// ```
        /// // Create a 0.5 x 0.5 instance
        /// var webViewPrefab = WebViewPrefab.Instantiate(0.5f, 0.5f);
        /// // Position the prefab how we want it
        /// webViewPrefab.transform.parent = transform;
        /// webViewPrefab.transform.localPosition = new Vector3(0, 0f, 0.5f);
        /// webViewPrefab.transform.LookAt(transform);
        /// // Load a URL once the prefab finishes initializing
        /// webViewPrefab.Initialized += (sender, e) => {
        ///     webViewPrefab.WebView.LoadUrl("https://vuplex.com");
        /// };
        /// ```
        /// </example>
        public static WebViewPrefab Instantiate(float width, float height) {

            return Instantiate(width, height, new WebViewOptions());
        }

        /// <summary>
        /// Like `Instantiate(float, float)`, except it also accepts an object
        /// of options flags that can be used to alter the generated webview's behavior.
        /// </summary>
        public static WebViewPrefab Instantiate(float width, float height, WebViewOptions options) {

            var prefabPrototype = (GameObject) Resources.Load("WebViewPrefab");
            var viewGameObject = (GameObject) Instantiate(prefabPrototype);
            var webViewPrefab = viewGameObject.GetComponent<WebViewPrefab>();
            webViewPrefab.Init(width, height, options);
            return webViewPrefab;
        }

        /// <summary>
        /// Asynchronously initializes the instance using the width and height
        /// set via the Unity editor.
        /// </summary>
        /// <remarks>
        /// You only need to call this method if you place a WebViewPrefab.prefab in your
        /// scene via the Unity editor and don't set its "Initial URL" property.
        /// You don't need to call this method if you set the "Initial URL" property in
        /// the editor or if you instantiate the prefab programmatically at runtime using
        /// `Instantiate()`. In those cases, `Init()` is called automatically for you.
        /// This method asynchronously initializes the `WebView` property, which is
        /// available for use after the `Initialized` event is raised.
        /// </remarks>
        public void Init() {

            // The top-level WebViewPrefab object's scale must be (1, 1),
            // so the scale that was set via the editor is transferred from WebViewPrefab
            // to WebViewPrefabResizer, and WebViewPrefab is moved to compensate
            // for how WebViewPrefabResizer is moved in _setViewSize.
            var localScale = transform.localScale;
            var localPosition = transform.localPosition;
            transform.localScale = new Vector3(1, 1, localScale.z);
            var offsetMagnitude = 0.5f * localScale.x;
            transform.localPosition = transform.localPosition + Quaternion.Euler(transform.localEulerAngles) * new Vector3(offsetMagnitude, 0, 0);
            Init(localScale.x, localScale.y);
        }

        /// <summary>
        /// Like `Init()`, except it initializes the instance to the specified
        /// width and height in Unity units.
        /// </summary>
        public virtual void Init(float width, float height) {

            Init(width, height, new WebViewOptions());
        }

        /// <summary>
        /// Like `Init(float, float)`, except it also accepts an object
        /// of options flags that can be used to alter the webview's behavior.
        /// </summary>
        public virtual void Init(float width, float height, WebViewOptions options) {

            _options = options;
            _viewResizer = transform.GetChild(0);
            _setViewSize(width, height);
            _initView();
            Web.CreateMaterial(viewportMaterial => {
                _view.SetMaterial(viewportMaterial);
                _initWebViewIfReady();
            });
            _videoRectPositioner = _getVideoRectPositioner();
            _initVideoLayer();
            if (options.disableVideo) {
                _videoLayerDisabled = true;
                _videoRectPositioner.gameObject.SetActive(false);
                _initWebViewIfReady();
            } else {
                Web.CreateVideoMaterial(videoMaterial => {
                    if (videoMaterial == null) {
                        _videoLayerDisabled = true;
                        _videoRectPositioner.gameObject.SetActive(false);
                    } else {
                        _videoLayer.SetMaterial(videoMaterial);
                    }
                    _initWebViewIfReady();
                });
            }
        }

        /// <summary>
        /// Destroys the instance and its children. Note that you don't have
        /// to call this method if you destroy the instance's parent with
        /// `Object.Destroy()`.
        /// </summary>
        public void Destroy() {

            UnityEngine.Object.Destroy(gameObject);
        }

        /// <summary>
        /// Resizes the prefab mesh and webview to the given
        /// dimensions in Unity units.
        /// </summary>
        public void Resize(float width, float height) {

            if (_webView != null) {
                _webView.Resize(width, height);
            }
            _setViewSize(width, height);
        }

        Vector2 _beginningDragPoint;
        Vector2 _beginningDragRatioPoint;
        bool _clickingEnabled = true;
        bool _clickIsPending;
        float _dragToScrollThreshold = 0.15f;
        WebViewOptions _options;
        Vector2 _previousDragPoint;
        bool _scrollingEnabled = true;
        static float _scrollSensitivity = 0.005f;
        ViewportMaterialView _videoLayer;
        bool _videoLayerDisabled;
        Transform _videoRectPositioner;
        protected WebViewPrefabView _view;
        Transform _viewResizer;
        protected IWebView _webView;
        GameObject _webViewGameObject;

        void _attachWebViewEventHandlers() {

            if (!_options.disableVideo) {
                _webView.VideoRectChanged += (sender, e) => _setVideoRect(e.Value);
            }
        }

        void Awake() {

            if (!String.IsNullOrEmpty(InitialUrl)) {
                Init();
            }

        }

        Vector2 _convertRatioPointToUnityUnits(Vector2 point) {

            var unityUnitsX = _viewResizer.transform.localScale.x * point.x;
            var unityUnitsY = _viewResizer.transform.localScale.y * point.y;
            return new Vector2(unityUnitsX, unityUnitsY);
        }

        /// <returns>
        /// A point where the x and y components are normalized
        /// values between 0 and 1.
        /// </returns>
        protected Vector2 _convertToScreenPosition(RaycastResult raycastResult) {

            var localPosition = _viewResizer.transform.InverseTransformPoint(raycastResult.worldPosition);
            return new Vector2(1 - localPosition.x, -1 * localPosition.y);
        }

        void _scrollIfNeeded(Vector2 scrollDelta, Vector2 mousePosition) {

            if (!_scrollingEnabled) {
                return;
            }
            if (scrollDelta.x == 0 && scrollDelta.y == 0) {
                // This can happen after the user drags the cursor off of the screen.
                return;
            }
            _webView.Scroll(scrollDelta, mousePosition);
        }

        ViewportMaterialView _getVideoLayer() {

            return _getVideoRectPositioner().GetComponentInChildren<ViewportMaterialView>();
        }

        Transform _getVideoRectPositioner() {

            var viewResizer = transform.GetChild(0);
            return viewResizer.Find("VideoRectPositioner");
        }

        protected virtual WebViewPrefabView _getView() {

            return transform.GetComponentInChildren<WebViewPrefabView>();
        }

    #if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        static void _handleScriptsReloadedInEditor() {

            var prefabs = Resources.FindObjectsOfTypeAll(typeof(WebViewPrefab)) as WebViewPrefab[];
            foreach (var prefab in prefabs) {
                if (prefab.gameObject.activeInHierarchy) {
                    prefab._reinitAfterScriptsReloaded();
                }
            }
        }
    #endif // UNITY_EDITOR

        void _initVideoLayer() {

            _videoLayer = _getVideoLayer();
        }

        void _initView() {

            _view = _getView();
            _view.BeganDrag += View_BeganDrag;
            _view.Dragged += View_Dragged;
            _view.PointerDown += View_PointerDown;
            _view.PointerUp += View_PointerUp;
            _view.Scrolled += View_Scrolled;
        }

        void _initWebViewIfReady() {

            if (_view.Texture == null || (!_videoLayerDisabled && _videoLayer.Texture == null)) {
                // Wait until both views' textures are ready.
                return;
            }
            _webView = Web.CreateWebView(_options.preferredPlugins);
            var webViewMonoBehaviour = _webView as MonoBehaviour;
            if (webViewMonoBehaviour != null) {
                // The IWebView is a MonoBehaviour, so set it as a child so that if this
                // object is persisted across scenes with DontDestroyOnLoad, so is
                // the webview.
                webViewMonoBehaviour.transform.parent = transform;
                // When scripts are reloaded in the editor, the _webView reference
                // is reset to null, so save a reference to the IWebView's gameObject
                // so that it can be used to recover the reference to the webview in that scenario.
                _webViewGameObject = webViewMonoBehaviour.gameObject;
            }
            _attachWebViewEventHandlers();
            _webView.Init(_view.Texture, _viewResizer.localScale.x, _viewResizer.localScale.y, _videoLayer.Texture);
            _setVideoRect(new Rect(0, 0, 0, 0));
            var handler = Initialized;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }

            if (!String.IsNullOrEmpty(InitialUrl)) {
                var url = InitialUrl.Trim();
                if (!url.Contains(":")) {
                    url = "http://" + url;
                }
                _webView.LoadUrl(url);
            }
        }

        void OnDestroy() {

            if (_webView != null) {
                _webView.Dispose();
            }
        }

        void _reinitAfterScriptsReloaded() {

            if (_webViewGameObject == null) {
                Debug.LogWarning("The IWebView is not a MonoBehaviour, so the reference to it cannot be recovered after the scripts were recompiled in the editor.");
                return;
            }
            _webView = _webViewGameObject.GetComponentInChildren<IWebView>();
            _attachWebViewEventHandlers();
            _initView();
            _initVideoLayer();
        }

        void _setVideoRect(Rect videoRect) {

            _view.SetCutoutRect(videoRect);
            // The origins of the prefab and the video rect are in their top-right
            // corners instead of their top-left corners.
            _videoRectPositioner.localPosition = new Vector3(
                1 - (videoRect.x + videoRect.width),
                -1 * videoRect.y,
                _videoRectPositioner.localPosition.z
            );
            _videoRectPositioner.localScale = new Vector3(videoRect.width, videoRect.height, _videoRectPositioner.localScale.z);

            // This code applies a cropping rect to the video layer's shader based on what part of the video (if any)
            // falls outside of the viewport and therefore needs to be hidden. Note that the dimensions here are divided
            // by the videoRect's width or height, because in the videoLayer shader, the width of the videoRect is 1
            // and the height is 1 (i.e. the dimensions are normalized).
            float videoRectXMin = Math.Max(0, - 1 * videoRect.x / videoRect.width);
            float videoRectYMin = Math.Max(0, -1 * videoRect.y / videoRect.height);
            float videoRectXMax = Math.Min(1, (1 - videoRect.x) / videoRect.width);
            float videoRectYMax = Math.Min(1, (1 - videoRect.y) / videoRect.height);
            var videoCropRect = Rect.MinMaxRect(videoRectXMin, videoRectYMin, videoRectXMax, videoRectYMax);
            if (videoCropRect == new Rect(0, 0, 1, 1)) {
                // The entire video rect fits within the viewport, so set the cropt rect to zero to disable it.
                videoCropRect = new Rect(0, 0, 0, 0);
            }
            _videoLayer.SetCropRect(videoCropRect);
        }

        void _setViewSize(float width, float height) {

            var depth = _viewResizer.localScale.z;
            _viewResizer.localScale = new Vector3(width, height, depth);
            var localPosition = _viewResizer.localPosition;
            // Set the view resizer so that its middle aligns with the middle of this parent class's gameobject.
            localPosition.x = width * -0.5f;
            _viewResizer.localPosition = localPosition;
        }

        void View_BeganDrag(object sender, EventArgs<PointerEventData> e) {

            _beginningDragRatioPoint = _convertToScreenPosition(e.Value.pointerCurrentRaycast);
            _beginningDragPoint = _convertRatioPointToUnityUnits(_beginningDragRatioPoint);
            _previousDragPoint = _beginningDragPoint;
        }

        void View_Dragged(object sender, EventArgs<PointerEventData> e) {

            if (e.Value.pointerCurrentRaycast.worldPosition == new Vector3(0, 0, 0)) {
                // This happens when the user drags off of the screen.
                return;
            }
            var point = _convertToScreenPosition(e.Value.pointerCurrentRaycast);
            var newDragPoint = _convertRatioPointToUnityUnits(point);
            var dragDelta = _previousDragPoint - newDragPoint;
            _scrollIfNeeded(dragDelta, _beginningDragRatioPoint);
            _previousDragPoint = newDragPoint;

            // Check whether to cancel a pending viewport click so that drag-to-scroll
            // doesn't unintentionally trigger a click.
            if (_clickIsPending) {
                var totalDragDelta = _beginningDragPoint - newDragPoint;
                var dragReachedScrollThreshold = Math.Abs(totalDragDelta.x) > _dragToScrollThreshold ||  Math.Abs(totalDragDelta.y) > _dragToScrollThreshold;
                if (dragReachedScrollThreshold) {
                    _clickIsPending = false;
                }
            }
        }

        protected virtual void View_PointerDown(object sender, EventArgs<PointerEventData> e) {

            if (_clickingEnabled) {
                _clickIsPending = true;
            }
        }

        protected virtual void View_PointerUp(object sender, EventArgs<PointerEventData> e) {

            if (!(_clickingEnabled && _clickIsPending)) {
                return;
            }
            _clickIsPending = false;
            var point = _convertToScreenPosition(e.Value.pointerPressRaycast);
            _webView.Click(point);
            if (Clicked != null) {
                Clicked(this, EventArgs.Empty);
            }
        }

        void View_Scrolled(object sender, EventArgs<PointerEventData> e) {

            var scaledScrollDelta = new Vector2(
                -e.Value.scrollDelta.x * _scrollSensitivity,
                -e.Value.scrollDelta.y * _scrollSensitivity
            );
            var mousePosition = _convertToScreenPosition(e.Value.pointerCurrentRaycast);
            _scrollIfNeeded(scaledScrollDelta, mousePosition);
        }
    }
}

