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

namespace Vuplex.WebView.Demos {

    /// <summary>
    /// Script that detects keys pressed on the hardware keyboard
    /// and emits the corresponding strings that
    /// can be passed to `IWebView.HandleKeyboardInput()`.
    /// </summary>
    class HardwareKeyboardListener : MonoBehaviour {

        public bool Enabled = false;

        public EventHandler<EventArgs<string>> InputReceived;

        public static HardwareKeyboardListener Instantiate() {

            return (HardwareKeyboardListener) new GameObject("HardwareKeyboardListener").AddComponent<HardwareKeyboardListener>();
        }

        void Update() {
            if (!Enabled) {
                return;
            }

            var handler = InputReceived;
            if (handler == null) {
                return;
            }
            foreach (var character in Input.inputString) {
                string characterString;
                switch (character) {
                    case '\b':
                        characterString = "Backspace";
                        break;
                    case '\n':
                        characterString = "Enter";
                        break;
                    default:
                        characterString = character.ToString();
                        break;
                }
                handler(this, new EventArgs<string>(characterString));
            }
            if (Input.GetKeyDown(KeyCode.UpArrow)) {
                handler(this, new EventArgs<string>("ArrowUp"));
            }
            if (Input.GetKeyDown(KeyCode.DownArrow)) {
                handler(this, new EventArgs<string>("ArrowDown"));
            }
            if (Input.GetKeyDown(KeyCode.RightArrow)) {
                handler(this, new EventArgs<string>("ArrowRight"));
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                handler(this, new EventArgs<string>("ArrowLeft"));
            }
        }
    }
}
