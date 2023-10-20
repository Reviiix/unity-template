﻿using UserInterface.MainMenus;

namespace Player
{
    /// <summary>
    /// This class manages player information
    /// </summary>
    public static class PlayerInformation
    {
        public static long PlayerID { get; private set; }
        public static string PlayerName { get; private set; }

        public static void Initialise()
        {
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
            SettingsMenu.OnNameChange += OnNameChange;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;

            PlayerID = saveData.playerID;
            PlayerName = saveData.playerName;
        }

        private static void OnNameChange(string name)
        {
            PlayerName = name;
        }
    }
}