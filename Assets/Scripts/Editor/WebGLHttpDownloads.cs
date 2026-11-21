using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UnityTemplate.EditorTools
{
    /// <summary>Allows downloads over plain HTTP for WebGL builds only, so a preview served from a LAN address loads its Addressables; iOS, Android and the saved Player Settings keep "Not allowed".</summary>
    [InitializeOnLoad]
    public class WebGLHttpDownloads : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string PreviousOptionKey = "UnityTemplate.WebGLHttpDownloads.PreviousOption";

        public int callbackOrder => 0;

        /// <summary>Puts the setting back if an earlier WebGL build stopped before restoring it.</summary>
        static WebGLHttpDownloads()
        {
            EditorApplication.delayCall += Restore;
        }

        /// <summary>Before a WebGL build: remembers the HTTP option and allows HTTP downloads for this build.</summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.WebGL || PlayerSettings.insecureHttpOption == InsecureHttpOption.AlwaysAllowed) return;

            SessionState.SetInt(PreviousOptionKey, (int)PlayerSettings.insecureHttpOption);
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
            EditorApplication.delayCall += Restore;
        }

        /// <summary>After the build: restores the remembered option.</summary>
        public void OnPostprocessBuild(BuildReport report)
        {
            Restore();
        }

        /// <summary>Restores the HTTP option remembered before a WebGL build, if one is pending.</summary>
        private static void Restore()
        {
            var previous = SessionState.GetInt(PreviousOptionKey, -1);
            if (previous < 0) return;

            PlayerSettings.insecureHttpOption = (InsecureHttpOption)previous;
            SessionState.EraseInt(PreviousOptionKey);
        }
    }
}
