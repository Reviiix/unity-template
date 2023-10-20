using System;
using Player;
using UserInterface.ConditionalMenus;

public static class HolidayManager
{
    public static Holiday CurrentHoliday { get; private set; } = Holiday.None;
    public static DateTime FirstOpen { get; private set; }
    public static int AmountOfYearsSinceFirstOpen { get; private set; } = 1;
    public static bool IsHoliday => CurrentHoliday != Holiday.None;
    public static bool IsAnniversary => DateTime.Today == new DateTime(FirstOpen.Year + AmountOfYearsSinceFirstOpen, FirstOpen.Month, FirstOpen.Day);

    public static void Initialise()
    {
        SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
        RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
        ProjectManager.OnApplicationOpen += OnApplicationOpen;
        AnniversaryPopUp.OnAnniversaryOfFirstOpen += OnAnniversaryOfFirstOpen;
    }
    
    private static void OnSaveDataLoaded(SaveSystem.SaveData saveData)
    {
        if (saveData == null) return;

        FirstOpen = saveData.FirstOpen;
        AmountOfYearsSinceFirstOpen = saveData.amountOfYearsSinceFirstOpen;
    }

    private static void OnConfigurationChanged(RemoteConfigurationManager.Configuration configuration)
    {
        var holiday = Holiday.None;
        if (Enum.TryParse<Holiday>(configuration.Holiday, out var returnVariable)) holiday = returnVariable;
        SetHoliday(holiday);
    }
    
    private static void OnApplicationOpen()
    {
        if (PlayerEngagement.TimesGameHasBeenOpened == 1) FirstOpen = DateTime.Today;
    }
        
    private static void OnAnniversaryOfFirstOpen()
    {
        AmountOfYearsSinceFirstOpen++;
    }

    private static void SetHoliday(Holiday holidayType)
    {
        CurrentHoliday = holidayType;
    }
    
    public enum Holiday
    {
        None,

        DoubleExperience,
        
        Christmas,
        Thanksgiving,
        Easter,
        Valentines,
        Halloween,
        BonfireNight,
        FourthOfJuly,
        
        Lent,
    }
}