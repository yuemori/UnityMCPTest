using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

namespace TicTacToe.Editor
{
    /// <summary>
    /// WebGL ビルド設定とビルド実行を支援するエディタスクリプト
    /// </summary>
    public static class WebGLBuildHelper
    {
        private const string TicTacToeScenePath = "Assets/TicTacToe/Scenes/TicTacToeScene.unity";
        private const string BuildOutputPath = "Build/WebGL";
        
        [MenuItem("TicTacToe/WebGL/1. Setup Build Settings", priority = 100)]
        public static void SetupBuildSettings()
        {
            // Build Settings にシーンを設定
            var scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene(TicTacToeScenePath, true)
            };
            EditorBuildSettings.scenes = scenes;
            
            Debug.Log($"[WebGL] Build Settings updated: {TicTacToeScenePath}");
        }
        
        [MenuItem("TicTacToe/WebGL/2. Configure WebGL Player Settings", priority = 101)]
        public static void ConfigureWebGLPlayerSettings()
        {
            // WebGL Player Settings
            PlayerSettings.companyName = "TicTacToe";
            PlayerSettings.productName = "TicTacToe";
            
            // WebGL 固有設定
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true; // GitHub Pages 対応
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.memorySize = 256;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
            PlayerSettings.runInBackground = true;
            
            // 解像度設定
            PlayerSettings.defaultScreenWidth = 960;
            PlayerSettings.defaultScreenHeight = 600;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            
            // Color Space (Linear推奨だがWebGL互換性のためGamma)
            PlayerSettings.colorSpace = ColorSpace.Linear;
            
            Debug.Log("[WebGL] Player Settings configured for WebGL deployment");
            Debug.Log("  - Compression: Gzip with Decompression Fallback");
            Debug.Log("  - Memory: 256MB");
            Debug.Log("  - Resolution: 960x600");
        }
        
        [MenuItem("TicTacToe/WebGL/3. Switch to WebGL Platform", priority = 102)]
        public static void SwitchToWebGL()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL)
            {
                Debug.Log("[WebGL] Already on WebGL platform");
                return;
            }
            
            Debug.Log("[WebGL] Switching to WebGL platform... This may take a while.");
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            Debug.Log("[WebGL] Platform switched to WebGL");
        }
        
        [MenuItem("TicTacToe/WebGL/4. Build WebGL", priority = 103)]
        public static void BuildWebGL()
        {
            // ビルド前にシーン確認
            if (EditorBuildSettings.scenes.Length == 0)
            {
                Debug.LogError("[WebGL] No scenes in Build Settings. Run 'Setup Build Settings' first.");
                return;
            }
            
            // 出力ディレクトリ作成
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), BuildOutputPath);
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }
            
            // ビルドオプション
            var buildOptions = new BuildPlayerOptions
            {
                scenes = new[] { TicTacToeScenePath },
                locationPathName = BuildOutputPath,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };
            
            Debug.Log($"[WebGL] Starting build to: {fullPath}");
            
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[WebGL] Build succeeded!");
                Debug.Log($"  - Output: {fullPath}");
                Debug.Log($"  - Size: {summary.totalSize / (1024 * 1024):F2} MB");
                Debug.Log($"  - Duration: {summary.totalTime.TotalSeconds:F1} seconds");
                
                // エクスプローラーで開く
                EditorUtility.RevealInFinder(fullPath);
            }
            else
            {
                Debug.LogError($"[WebGL] Build failed: {summary.result}");
                Debug.LogError($"  - Errors: {summary.totalErrors}");
                Debug.LogError($"  - Warnings: {summary.totalWarnings}");
            }
        }
        
        [MenuItem("TicTacToe/WebGL/-- Run All Steps --", priority = 200)]
        public static void RunAllSteps()
        {
            Debug.Log("[WebGL] Running all setup steps...");
            
            SetupBuildSettings();
            ConfigureWebGLPlayerSettings();
            SwitchToWebGL();
            
            Debug.Log("[WebGL] Setup complete! Run 'Build WebGL' when platform switch is finished.");
        }
        
        [MenuItem("TicTacToe/WebGL/Open Build Folder", priority = 300)]
        public static void OpenBuildFolder()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), BuildOutputPath);
            if (Directory.Exists(fullPath))
            {
                EditorUtility.RevealInFinder(fullPath);
            }
            else
            {
                Debug.LogWarning($"[WebGL] Build folder does not exist: {fullPath}");
            }
        }
    }
}
