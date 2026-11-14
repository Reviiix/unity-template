namespace UnityTemplate.Achievements
{
    /// <summary>Every achievement id; saves store these names, so rename an entry only together with a save migration.</summary>
    public enum Achievement
    {
        PlayForOneHourConsecutively,
        PlayForOneHour,
        PlayForFiveHours,
        PlayForTenHours,
        PlayForTwentyFiveHours,
        PlayForFiftyHours,
        PlayForOneHundredHours,

        OpenTheAppOnce,
        OpenTheAppFiveTimes,
        OpenTheAppTenTimes,
        OpenTheAppTwentyFiveTimes,
        OpenTheAppFiftyTimes,
        OpenTheAppOneHundredTimes,

        ReachLevelTwo,
        ReachLevelFive,
        ReachLevelTen,
        ReachLevelTwentyFive,
        ReachLevelFifty,
        ReachLevelOneHundred,

        OneHundredCredits,
        TwoHundredAndFiftyCredits,
        FiveHundredCredits,
        OneThousandCredits,
        TenThousandCredits,
        OneHundredThousandCredits,

        OneHundredPremiumCredits,
        TwoHundredAndFiftyPremiumCredits,
        FiveHundredPremiumCredits,
        OneThousandPremiumCredits,
        TenThousandPremiumCredits,
        OneHundredThousandPremiumCredits,

        HighScore,
        ConsecutivePlayTimeInSeconds,
        TotalPlayTimeInSeconds,
        ExperienceGained,
        LevelsComplete,
    }
}
