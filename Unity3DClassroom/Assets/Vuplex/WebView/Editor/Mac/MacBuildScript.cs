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
#if UNITY_STANDALONE_OSX
#pragma warning disable CS0618
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine.Rendering;

namespace Vuplex.WebView {

    public class MacBuildScript : IPreprocessBuild {

        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildTarget buildTarget, string buildPath) {

            var graphicsApis = PlayerSettings.GetGraphicsAPIs(buildTarget);
            if (!graphicsApis.ToList().Contains(GraphicsDeviceType.Metal)) {
                throw new BuildFailedException("Unsupported Graphics API: Vuplex 3D WebView for macOS requires Metal. Please go to Player Settings and set \"Graphics APIs for Mac\" to Metal.");
            }
        }
    }
}
#endif // UNITY_STANDALONE_OSX
