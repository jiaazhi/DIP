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

namespace Vuplex.WebView {
    /// <summary>
    /// Static utility methods used internally by 3D WebView.
    /// </summary>
    static class Utils {
        /// <summary>
        /// Enables gamma correction in the Viewport shader When linear color space is
        /// enabled in order to prevent the texture from appearing bright and washed
        /// out due to incorrect gamma.
        /// </summary>
        public static void EnableGammaCorrection(Material material) {

            if (QualitySettings.activeColorSpace == ColorSpace.Linear) {
                material.SetFloat("_EnableGammaCorrection", 1.0f);
            }
        }
    }
}
