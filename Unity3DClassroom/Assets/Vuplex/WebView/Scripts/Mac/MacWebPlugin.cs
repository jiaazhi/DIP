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
using UnityEngine;

namespace Vuplex.WebView {
    /// <summary>
    /// The macOS `IWebPlugin` implementation.
    /// </summary>
    class MacWebPlugin : MonoBehaviour, IWebPlugin {

        public static MacWebPlugin Instance {
            get {
                if (_instance == null) {
                    _instance = (MacWebPlugin) new GameObject("MacWebPlugin").AddComponent<MacWebPlugin>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public void ClearAllData() {

            MacWebView.ClearAllData();
        }

        public void CreateTexture(float width, float height, Action<Texture2D> callback) {

            MacWebView.CreateTexture(width, height, callback);
        }

        public void CreateMaterial(Action<Material> callback) {

            CreateTexture(1, 1, texture => {
                var material = Resources.Load<Material>("DefaultViewportMaterial");
                Utils.EnableGammaCorrection(material);
                material.mainTexture = texture;
                callback(material);
            });
        }

        public void CreateVideoMaterial(Action<Material> callback) {

            callback(null);
        }

        public virtual IWebView CreateWebView() {

            return MacWebView.Instantiate();
        }

        public void SetStorageEnabled(bool enabled) {

            MacWebView.SetStorageEnabled(enabled);
        }

        public void SetUserAgent(bool mobile) {

            MacWebView.GloballySetUserAgent(mobile);
        }

        public void SetUserAgent(string userAgent) {

            MacWebView.GloballySetUserAgent(userAgent);
        }

        static MacWebPlugin _instance;

        void OnApplicationQuit() {

            MacWebView.TerminatePlugin();
        }

        void OnValidate() {

            MacWebView.ValidateGraphicsApi();
        }
    }
}
#endif
