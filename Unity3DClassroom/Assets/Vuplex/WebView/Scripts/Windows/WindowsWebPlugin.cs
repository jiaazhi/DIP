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
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vuplex.WebView {
    /// <summary>
    /// The Windows `IWebPlugin` implementation.
    /// </summary>
    class WindowsWebPlugin : MonoBehaviour, IWebPlugin {

        static WindowsWebPlugin() {

            WindowsWebView.EnableDebugLogging();
        }

        public static WindowsWebPlugin Instance {
            get {
                if (_instance == null) {
                    _instance = (WindowsWebPlugin) new GameObject("WindowsWebPlugin").AddComponent<WindowsWebPlugin>();
                    DontDestroyOnLoad(_instance.gameObject);
                }
                return _instance;
            }
        }

        public void ClearAllData() {

            WindowsWebView.ClearAllData();
        }

        public void CreateTexture(float width, float height, Action<Texture2D> callback) {

            int nativeWidth = (int)(width * Config.NumberOfPixelsPerUnityUnit);
            int nativeHeight = (int)(height * Config.NumberOfPixelsPerUnityUnit);
            var texture = new Texture2D(
                nativeWidth,
                nativeHeight,
                TextureFormat.RGBA32,
                false,
                false
            );
            // Invoke the callback asynchronously in order to match the async
            // behavior that's required for Android.
            Dispatcher.RunOnMainThread(() => callback(texture));
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

            return WindowsWebView.Instantiate();
        }

        public void SetStorageEnabled(bool enabled) {

            WindowsWebView.SetStorageEnabled(enabled);
        }

        public void SetUserAgent(bool mobile) {

            WindowsWebView.GloballySetUserAgent(mobile);
        }

        public void SetUserAgent(string userAgent) {

            WindowsWebView.GloballySetUserAgent(userAgent);
        }

        static WindowsWebPlugin _instance;

        void OnApplicationQuit() {

            WindowsWebView.TerminatePlugin();
        }

        void OnValidate() {

            WindowsWebView.ValidateGraphicsApi();
        }
    }
}
#endif
