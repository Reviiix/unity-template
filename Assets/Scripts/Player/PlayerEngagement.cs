using System;

namespace Player
{
    /// <summary>
    /// This class tracks the players engagement with the application.
    /// Amount opened, time played etc
    /// </summary>
    public static class PlayerEngagement
    {
        public static DateTime LastTimeAppWasOpen { get; private set; } 
        public static int ConsecutiveDailyOpens { get; private set; }
        public static TimeSpan TotalPlayTime{ get; private set; } 
        private static bool OpenedYesterday => LastTimeAppWasOpen.Date == DateTime.Today.Subtract(new TimeSpan(1,0,0,0)).Date;
        public static bool IsRepeatOpenToday => DateTime.Today.Date == LastTimeAppWasOpen.Date;
        public static Action<int> OnDailyBonusOpen;
        private static int _consecutiveDays = 1;
        public static readonly int DailyBonusRewardCredits = 10 * _consecutiveDays;
        public static readonly int DailyBonusRewardPremiumCredits = DailyBonusRewardCredits / 10 * _consecutiveDays;
        public const int TutorialRewardCredits = 100;
        public const int TutorialRewardPremiumCredits = TutorialRewardCredits / 10;
        public static readonly int AnniversaryRewardCredits = 100 * HolidayManager.AmountOfYearsSinceFirstOpen;
        public static readonly int AnniversaryRewardPremiumCredits = AnniversaryRewardCredits / 10;
        private static int _timesGameHasBeenOpened;
        public static int TimesGameHasBeenOpened
        {
            get => _timesGameHasBeenOpened;
            // ReSharper disable once ValueParameterNotUsed
            private set => _timesGameHasBeenOpened++;
        }

        public static void Initialise()
        {
            ProjectManager.OnApplicationOpen += OnApplicationOpen;
            SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        }
    
        private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
        {
            if (saveData == null) return;
            
            _timesGameHasBeenOpened = saveData.timesGameHasBeenOpened;
            LastTimeAppWasOpen = saveData.LastTimeAppWasOpen;
            ConsecutiveDailyOpens = saveData.consecutiveDailyOpens;
            TotalPlayTime = saveData.TotalPlayTime;
            ConsecutiveDailyOpens = saveData.consecutiveDailyOpens;
        }

        private static void OnApplicationOpen()
        {
            TimesGameHasBeenOpened++;
        
            DebuggingAid.DebugLogManager.Log("App has been opened " + TimesGameHasBeenOpened + " times.");
            
            ProjectManager.OnApplicationOpen -= OnApplicationOpen;
        
            if (IsRepeatOpenToday) return;
        
            if (OpenedYesterday) _consecutiveDays++;
        }
    }
}
