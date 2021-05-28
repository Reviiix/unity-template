using System;
using Player;

public static class HolidayManager
{
    public static Holiday CurrentHoliday { get; private set; } = Holiday.None;
    public static bool IsHoliday => CurrentHoliday != Holiday.None;
    public static bool IsUserBirthday => DateTime.Today == PlayerInformation.Birthday;

    public static void Initialise()
    {
        RemoteConfigurationManager.OnConfigurationChanged += OnConfigurationChanged;
    }

    private static void OnConfigurationChanged(RemoteConfigurationManager.Configuration configuration)
    {
        var holiday = (Holiday)Enum.Parse(typeof(Holiday), configuration.Holiday);

        if (holiday == null) holiday = Holiday.None;
        
        SetHoliday(holiday);
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