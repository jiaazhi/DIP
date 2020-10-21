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

namespace Vuplex.WebView {

    /// <summary>
    /// `Web` is the top-level static class for the 3D WebView plugin.
    /// It contains static methods for configuring the module and creating resources.
    /// </summary>
    public static class Web {

        /// <summary>
        /// Clears all data that persists between webview instances,
        /// including cookies, storage, and cached resources.
        /// </summary>
        /// <remarks>
        /// On Windows and macOS, this method can only be called prior to
        /// creating any webviews.
        /// </remarks>
        public static void ClearAllData() {

            _pluginFactory.GetPlugin().ClearAllData();
        }

        /// <summary>
        /// Creates a material and texture that a webview can use for rendering.
        /// </summary>
        /// <remarks>
        /// This method is asynchronous
        /// in order to allow textures to be created on the render thread, and the
        /// provided callback is called once the material has been created.
        ///
        /// Note that `WebViewPrefab` and `CanvasWebViewPrefab` take care of material creation for you, so you only need
        /// to call this method directly if you need to create an `IWebView` instance outside of a prefab with
        /// `Web.CreateWebView()`.
        /// </remarks>
        public static void CreateMaterial(Action<Material> callback) {

            _pluginFactory.GetPlugin().CreateMaterial(callback);
        }

        /// <summary>
        /// Like `CreateMaterial`, except it creates a material that a webview
        /// can use for rendering video. If the platform doesn't need a separate
        /// material and texture for video, this method returns `null`.
        /// </summary>
        /// <remarks>
        /// Currently, iOS is the only platform that always uses a separate texture
        /// for video. Android only uses a separate video texture on versions of Android
        /// older than 6.0. For other platforms, video content is always integrated into
        /// the main texture.
        /// </remarks>
        public static void CreateVideoMaterial(Action<Material> callback) {

            _pluginFactory.GetPlugin().CreateVideoMaterial(callback);
        }

        /// <summary>
        /// Creates a special texture that a webview can use for rendering, using the given
        /// width and height in Unity units (not pixels). The webview plugin automatically
        /// resizes a texture when it initializes or resizes a webview, so in practice, you
        /// can simply use the dimensions of 1x1 like `CreateTexture(1, 1, callback)`.
        /// </summary>
        /// <remarks>
        /// This method is asynchronous
        /// in order to allow textures to be created on the render thread, and the
        /// provided callback is called once the texture has been created.
        ///
        /// Note that the `WebViewPrefab` takes care of texture creation for you, so you only need
        /// to call this method directly if you need to create an `IWebView` instance outside of a prefab with
        /// `Web.CreateWebView()`.
        /// </remarks>
        public static void CreateTexture(float width, float height, Action<Texture2D> callback) {

            _pluginFactory.GetPlugin().CreateTexture(width, height, callback);
        }

        /// <summary>
        /// Creates a new webview in a platform-agnostic way. After a webview
        /// is created, it must be initialized by calling one of its `Init()`
        /// methods.
        /// </summary>
        /// <remarks>
        /// Note that `WebViewPrefab` takes care of creating and managing
        /// an `IWebView` instance for you, so you only need to call this method directly
        /// if you need to create an `IWebView` instance outside of a prefab
        /// (for example, to connect it to your own custom GameObject).
        /// </remarks>
        /// <example>
        /// <![CDATA[
        /// ```cs
        /// Web.CreateMaterial(material => {
        ///     // Set the material attached to this GameObject so that it can display the web content.
        ///     GetComponent<Renderer>().material = material;
        ///     var webView = Web.CreateWebView();
        ///     webView.Init(material.mainTexture, 1, 1);
        ///     webView.LoadUrl("https://vuplex.com");
        /// });
        /// ```
        /// ]]>
        /// </example>
        public static IWebView CreateWebView() {

            return _pluginFactory.GetPlugin().CreateWebView();
        }

        /// <summary>
        /// Like `CreateWebView()`, except an array of preferred plugin types can be
        /// provided to override which 3D WebView plugin is used in the case where
        /// multiple plugins are installed for the same build platform.
        /// </summary>
        /// <remarks>
        /// Currently, Android is the only platform that supports multiple 3D WebView
        /// plugins: `WebPluginType.Android` and `WebPluginType.AndroidGecko`. If both
        /// plugins are installed in the same project, `WebPluginType.AndroidGecko` will be used by default.
        /// However, you can override this to force `WebPluginType.Android` to be used instead by passing
        /// `new WebPluginType[] { WebPluginType.Android }`.
        /// </remarks>
        public static IWebView CreateWebView(WebPluginType[] preferredPlugins) {

            return _pluginFactory.GetPlugin(preferredPlugins).CreateWebView();
        }

        /// <summary>
        /// Controls whether data like cookies, localStorage, and cached resources
        /// is persisted between webview instances. The default is `true`, but this
        /// can be set to `false` to achieve an "incognito mode".
        /// </summary>
        /// <remarks>
        /// On Windows and macOS, this method can only be called prior to
        /// creating any webviews.
        /// </remarks>
        public static void SetStorageEnabled(bool enabled) {

            _pluginFactory.GetPlugin().SetStorageEnabled(enabled);
        }

        /// <summary>
        /// By default, webviews use a User-Agent that looks that of a desktop
        /// computer so that servers return the desktop versions of websites.
        /// If you instead want the mobile versions of websites, you can invoke
        /// this method with `true` to use the User-Agent for a mobile device.
        /// </summary>
        /// <remarks>
        /// On Windows and macOS, this method can only be called prior to
        /// creating any webviews.
        /// </remarks>
        public static void SetUserAgent(bool mobile) {

            _pluginFactory.GetPlugin().SetUserAgent(mobile);
        }

        /// <summary>
        /// Configures the module to use a custom User-Agent string.
        /// </summary>
        /// <remarks>
        /// On Windows and macOS, this method can only be called prior to
        /// creating any webviews.
        /// </remarks>
        public static void SetUserAgent(string userAgent) {

            _pluginFactory.GetPlugin().SetUserAgent(userAgent);
        }

        static internal void SetPluginFactory(WebPluginFactory pluginFactory) {

            _pluginFactory = pluginFactory;
        }

        static WebPluginFactory _pluginFactory = new WebPluginFactory();
    }
}
