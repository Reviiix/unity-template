using System;
using System.IO;
using UnityEngine;
using UnityTemplate.Core;

namespace UnityTemplate.Tests
{
    /// <summary>Creates a SaveService in a throwaway folder so tests never touch the developer's real save.</summary>
    public sealed class TestSaveFolder : IDisposable
    {
        public string Path { get; } = System.IO.Path.Combine(Application.temporaryCachePath, "SaveTests", Guid.NewGuid().ToString("N"));

        /// <summary>Creates the folder.</summary>
        public TestSaveFolder()
        {
            Directory.CreateDirectory(Path);
        }

        /// <summary>Returns a new SaveService that saves into this folder.</summary>
        public SaveService CreateSaveService()
        {
            return new SaveService(Path);
        }

        /// <summary>Deletes the folder and everything saved in it.</summary>
        public void Dispose()
        {
            if (Directory.Exists(Path)) Directory.Delete(Path, true);
        }
    }
}
