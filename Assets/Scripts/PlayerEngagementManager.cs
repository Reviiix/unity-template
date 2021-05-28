using System;
using Player;

public static class PlayerEngagementManager
{
    private static DateTime LastTimeAppWasOpen => PlayerInformation.LastTimeAppWasOpen;
    private static bool OpenedYesterday => LastTimeAppWasOpen.Date == DateTime.Today.Subtract(new TimeSpan(1,0,0,0)).Date;
    public static bool IsRepeatOpenToday => DateTime.Today.Date == LastTimeAppWasOpen.Date;
    public static Action<int> OnDailyBonusOpen;
    private static int _consecutiveDays;
    public static readonly int DailyBonusReward = 100 * _consecutiveDays;
    public const int WelcomeRewardCredits = 100;
    public const int WelcomeRewardPremiumCredits = 50;
    public const int BirthdayReward = 1000;
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
        SaveSystem.OnSaveDataLoaded += LoadEngagementStatistics;
        _consecutiveDays = PlayerInformation.ConsecutiveDailyOpens;
    }
    
    private static void LoadEngagementStatistics(SaveSystem.SaveData saveData)
    {
        if (saveData == null) return;
            
        _timesGameHasBeenOpened = saveData.TimesGameHasBeenOpenedSave;
    }

    private static void OnApplicationOpen()
    {
        TimesGameHasBeenOpened++;
        
        DebuggingAid.Debugging.DisplayDebugMessage("App has been opened " + TimesGameHasBeenOpened + " times.");
        
        if (IsRepeatOpenToday) return;
        
        if (OpenedYesterday) _consecutiveDays++;
        
        OnDailyBonusOpen?.Invoke(_consecutiveDays);

        ProjectManager.OnApplicationOpen -= OnApplicationOpen;
    }
}
