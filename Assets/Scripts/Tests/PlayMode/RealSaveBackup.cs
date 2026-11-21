using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityTemplate.Tests.PlayMode
{
    /// <summary>Moves the developer's real save aside before the PlayMode run and puts it back afterwards (deleting any save the tests or quitting Play mode left behind), so a run leaves the save folder exactly as it found it.</summary>
    public class RealSaveBackup : IPrebuildSetup, IPostBuildCleanup
    {
        public const string SaveFileName = "save.json";
        private const string BackupFolderName = "PlayModeTestsSaveBackup";

        private static string BackupFolder => Path.Combine(Application.temporaryCachePath, BackupFolderName);

        /// <summary>Before the run: moves the real save files into the backup folder. If a backup is already there, an earlier run never finished, so it holds the real save and the current files are test leftovers.</summary>
        public void Setup()
        {
            if (Directory.Exists(BackupFolder))
            {
                DeleteSaves();
                return;
            }
            Directory.CreateDirectory(BackupFolder);
            foreach (var path in SaveFiles()) File.Move(path, Path.Combine(BackupFolder, Path.GetFileName(path)));
        }

        /// <summary>After the run, once Play mode has exited: deletes whatever save exists and restores the backed-up files, if any.</summary>
        public void Cleanup()
        {
            DeleteSaves();
            if (!Directory.Exists(BackupFolder)) return;
            foreach (var path in Directory.GetFiles(BackupFolder)) File.Move(path, Path.Combine(Application.persistentDataPath, Path.GetFileName(path)));
            Directory.Delete(BackupFolder, true);
        }

        /// <summary>Deletes every save file (the save, its backup and any unfinished temporary file).</summary>
        public static void DeleteSaves()
        {
            foreach (var path in SaveFiles()) File.Delete(path);
        }

        /// <summary>Returns the save files in the save folder, or none when the folder doesn't exist yet.</summary>
        private static string[] SaveFiles()
        {
            var folder = Application.persistentDataPath;
            return Directory.Exists(folder) ? Directory.GetFiles(folder, SaveFileName + "*") : new string[0];
        }
    }
}
