using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace FootballSim.Build
{
    public static class BuildPipeline
    {
        public static void BuildWindows()
        {
            string buildPath = "Builds/Windows/FootballSim.exe";
            BuildPlayerOptions options = new BuildPlayerOptions
            {
                scenes = GetEnabledScenes(),
                locationPathName = buildPath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                Debug.LogError($"Build failed: {report.summary.result}");
            }
            else
            {
                Debug.Log("Build succeeded.");
            }
        }

        private static string[] GetEnabledScenes()
        {
            var scenes = EditorBuildSettings.scenes;
            var enabled = new List<string>();
            foreach (var scene in scenes)
            {
                if (scene.enabled)
                {
                    enabled.Add(scene.path);
                }
            }

            return enabled.ToArray();
        }
    }
}
