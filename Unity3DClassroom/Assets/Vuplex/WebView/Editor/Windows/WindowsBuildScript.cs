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
#if UNITY_STANDALONE_WIN
#pragma warning disable CS0618
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Vuplex.WebView {
    /// <summary>
    /// Windows build script that copies the Chromium plugin executable's files to the
    /// required location in the built application folder.
    /// </summary>
    public class WindowsBuildScript : IPreprocessBuild {

        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildTarget buildTarget, string buildPath) {

            if (buildTarget != BuildTarget.StandaloneWindows) {
                return;
            }
            var graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows);
            if (!graphicsApis.ToList().Contains(GraphicsDeviceType.Direct3D11)) {
                throw new BuildFailedException("Unsupported Graphics API: Vuplex 3D WebView for Windows requires Direct3D11.");
            }
        }

        [PostProcessBuild(700)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject) {

            if (!(target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)) {
                return;
            }
            var buildPluginDirectoryPath = _getBuiltPluginDirectoryPath(pathToBuiltProject);
            var sourceChromiumDirectory = _getChromiumDirectoryPath();
            var destinationChromiumDirectory = Path.Combine(buildPluginDirectoryPath, CHROMIUM_DIRECTORY_NAME);
            _copyAndReplaceDirectory(sourceChromiumDirectory, destinationChromiumDirectory);
        }

        const string DLL_FILE_NAME = "VuplexWebViewWindows.dll";
        const string CHROMIUM_DIRECTORY_NAME = "VuplexWebViewChromium";

        static void _copyAndReplaceDirectory(string srcPath, string dstPath) {

            if (Directory.Exists(dstPath)) {
                Directory.Delete(dstPath, true);
            }
            if (File.Exists(dstPath)) {
                File.Delete(dstPath);
            }
            Directory.CreateDirectory(dstPath);

            foreach (var file in Directory.GetFiles(srcPath)) {
                File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));
            }
            foreach (var dir in Directory.GetDirectories(srcPath)) {
                _copyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
            }
        }

        static string _getBuiltPluginDirectoryPath(string pathToBuiltProject) {

            var productName = Path.GetFileNameWithoutExtension(pathToBuiltProject);
            var buildDirectoryPath = _getParentDirectoryOfFile(pathToBuiltProject, '/');
            var expectedPluginDirectoryPath = _pathCombine(new string[] { buildDirectoryPath, productName + "_Data", "Plugins" });
            var expectedVuplexPluginFilePath = Path.Combine(expectedPluginDirectoryPath, DLL_FILE_NAME);
            if (File.Exists(expectedVuplexPluginFilePath)) {
                return expectedPluginDirectoryPath;
            }
            // The DLL isn't at the expected path, so let's try to find it in the built project.
            var files = Directory.GetFiles(buildDirectoryPath, DLL_FILE_NAME, SearchOption.AllDirectories);
            if (files.Count() == 1) {
                return _getParentDirectoryOfFile(files[0], Path.DirectorySeparatorChar);
            }
            var errorMessage = String.Format("Vuplex.WebView build error: unable to locate the {0} file in the built project folder. It's not in the expected location ({1}), and {2} instances of the file were found in the built project folder.", DLL_FILE_NAME, expectedVuplexPluginFilePath, files.Count());
            throw new Exception(errorMessage);
        }

        static string _getChromiumDirectoryPath() {

            var expectedPath = _pathCombine(new string[] { Application.dataPath, "Vuplex", "WebView", "Plugins", "Windows", CHROMIUM_DIRECTORY_NAME });
            if (Directory.Exists(expectedPath)) {
                return expectedPath;
            }
            // The Chromium directory isn't in the default location (Assets/Vuplex/WebView/Plugins/Windows/VuplexWebViewChromium).
            // So, let's try to find where it in the Assets directory.
            var directories = Directory.GetDirectories(Application.dataPath, CHROMIUM_DIRECTORY_NAME, SearchOption.AllDirectories);
            if (directories.Count() == 1) {
                return directories[0];
            }
            var errorMessage = String.Format("Vuplex.WebView build error: unable to locate the {0} directory in the Assets folder. It's not in the default location ({1}), and {2} instances of the directory were found in Assets folder.", CHROMIUM_DIRECTORY_NAME, expectedPath, directories.Count());
            throw new Exception(errorMessage);
        }

        static string _getParentDirectoryOfFile(string filePath, char pathSeparator) {

            var pathComponents = filePath.Split(new char[] { pathSeparator }).ToList();
            return String.Join(Path.DirectorySeparatorChar.ToString(), pathComponents.GetRange(0, pathComponents.Count - 1).ToArray());
        }

        /// <summary>
        /// A polyfill for `Path.Combine(string[])`, which isn't present in .NET 2.0.
        /// </summary>
        static string _pathCombine(string[] pathComponents) {

            if (pathComponents.Length == 0) {
                return "";
            }
            if (pathComponents.Length == 1) {
                return pathComponents[0];
            }
            var path = pathComponents[0];
            for (var i = 1; i < pathComponents.Length; i++) {
                path = System.IO.Path.Combine(path, pathComponents[i]);
            }
            return path;
        }
    }
}
#endif // UNITY_STANDALONE_WIN
