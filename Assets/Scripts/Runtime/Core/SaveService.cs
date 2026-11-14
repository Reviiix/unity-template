using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace UnityTemplate.Core
{
    /// <summary>Loads and saves the player's progress as JSON in persistentDataPath, writing atomically and keeping a backup copy.</summary>
    public class SaveService
    {
        private const string FileName = "save.json";
        private readonly string filePath;
        private readonly string backupFilePath;
        private readonly string temporaryFilePath;
        private readonly List<ISaveable> saveables = new();

        public bool IsLoaded { get; private set; }
        public event Action DataLoaded;

        /// <summary>Creates the service for a save folder; the game passes Application.persistentDataPath and tests pass a temporary folder.</summary>
        public SaveService(string folder)
        {
            filePath = Path.Combine(folder, FileName);
            backupFilePath = filePath + ".backup";
            temporaryFilePath = filePath + ".tmp";
        }

        /// <summary>Adds a service whose state is read on Load and written on Save.</summary>
        public void Register(ISaveable saveable)
        {
            saveables.Add(saveable);
        }

        /// <summary>Reads the save file (or its backup) and hands the data to every registered service; a missing or unreadable file starts a new game.</summary>
        public void Load()
        {
            var data = ReadSaveFile(filePath) ?? ReadSaveFile(backupFilePath) ?? new SaveData();
            foreach (var saveable in saveables)
            {
                saveable.Load(data);
            }
            IsLoaded = true;
            DataLoaded?.Invoke();
        }

        /// <summary>Collects every registered service's state and writes it to disk; does nothing until Load has run, so defaults never overwrite a save.</summary>
        public void Save()
        {
            if (!IsLoaded) return;

            var data = new SaveData();
            foreach (var saveable in saveables)
            {
                saveable.Save(data);
            }

            try
            {
                WriteAtomically(JsonUtility.ToJson(data));
            }
            catch (Exception exception)
            {
                Debug.LogError("Saving failed: " + exception);
            }
        }

        /// <summary>Deletes the save files and resets every registered service to a new game; listeners of DataLoaded refresh their display.</summary>
        public void DeleteAll()
        {
            foreach (var path in new[] { filePath, backupFilePath, temporaryFilePath })
            {
                if (File.Exists(path)) File.Delete(path);
            }

            var newGame = new SaveData();
            foreach (var saveable in saveables)
            {
                saveable.Load(newGame);
            }
            DataLoaded?.Invoke();
        }

        /// <summary>Reads and parses one save file, returning null when it is missing, unreadable or invalid.</summary>
        private static SaveData ReadSaveFile(string path)
        {
            if (!File.Exists(path)) return null;

            try
            {
                var data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path, Encoding.UTF8));
                return data is { version: > 0 } ? data : null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"Could not read save file {path}: {exception.Message}");
                return null;
            }
        }

        /// <summary>Writes to a temporary file and swaps it in, keeping the previous save as the backup.</summary>
        private void WriteAtomically(string json)
        {
            File.WriteAllText(temporaryFilePath, json, Encoding.UTF8);

            if (File.Exists(filePath))
            {
                File.Replace(temporaryFilePath, filePath, backupFilePath);
            }
            else
            {
                File.Move(temporaryFilePath, filePath);
            }
        }
    }
}
