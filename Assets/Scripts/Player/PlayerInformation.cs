﻿using System;
using UserInterface.ConditionalMenus;
using UserInterface.MainMenus;

namespace Player
{
    public static class PlayerInformation
    {
        public static long PlayerID { get; private set; }
        public static string PlayerName { get; private set; }

        public static void Initialise()
        {
            FirstOpenPopUpMenu.OnPlayerInformationSubmitted += OnPlayerInformationSubmitted;
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
            SettingsMenu.OnNameChange += OnNameChange;
        }

        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;

            PlayerID = saveData.PlayerID;
            PlayerName = saveData.PlayerName;
        }
        
        private static void OnPlayerInformationSubmitted(string name, DateTime birthday)
        {
            PlayerID = 111; //TODO custom identifier
            PlayerName = name;
        }
        
        private static void OnNameChange(string name)
        {
            PlayerName = name;
        }
    }
}