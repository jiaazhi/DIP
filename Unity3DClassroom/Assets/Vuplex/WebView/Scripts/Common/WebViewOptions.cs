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

namespace Vuplex.WebView {

    /// <summary>
    /// Options that can be passed to the `WebViewPrefab.Instantiate()` to alter the behavior of
    /// the WebView created.
    /// </summary>
    public struct WebViewOptions {

        /// <summary>
        /// Videos are enabled by default, but you can disable them with this option
        /// if you know you will not need to render videos. Setting this option to `true` will prevent the prefab
        /// from allocating a second texture for video and prevent it from allocating
        /// resources for video playback.
        /// </summary>
        public bool disableVideo;

        /// <summary>
        /// 3D WebView automatically selects which native plugin to use based on
        /// the build platform and which plugins are installed in the project.
        /// However, if you have multiple plugins installed for a single platform,
        /// this option can be used to specify which plugin to use in order to override
        /// the default behavior.
        /// </summary>
        /// <remarks>
        /// Currently, Android is the only platform that supports multiple 3D WebView
        /// plugins: `WebPluginType.Android` and `WebPluginType.AndroidGecko`. If both
        /// plugins are installed in the same project, `WebPluginType.AndroidGecko` will be used by default.
        /// However, you can override this to force `WebPluginType.Android` to be used instead by specifying
        /// `preferredPlugins = new WebPluginType[] { WebPluginType.Android }`.
        /// </remarks>
        public WebPluginType[] preferredPlugins;
    }
}
