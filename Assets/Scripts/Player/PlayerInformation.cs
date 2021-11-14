using UserInterface.MainMenus;

namespace Player
{
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

            PlayerID = saveData.PlayerID;
            PlayerName = saveData.PlayerName;
        }

        private static void OnNameChange(string name)
        {
            PlayerName = name;
        }
    }
}