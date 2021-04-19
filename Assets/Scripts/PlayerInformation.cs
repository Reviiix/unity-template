using System;
using Experience;
using UnityEngine;

[Serializable]
public static class PlayerInformation
{
    public static int PlayerID { get; private set; }
    public static int Level { get; private set; }
    public static long TotalExperience { get; private set; }
    public static long CurrentExperience { get; private set; }

    public static void Initialise()
    {
        LoadPlayerInformation();
        OnStart();
    }

    private static void OnStart()
    {
        ExperienceManager.OnExperienceChange += OnExperienceChange;
        ExperienceManager.OnLevelChange += OnLevelChange;
    }

    private static void OnQuit()
    {
        ExperienceManager.OnExperienceChange -= OnExperienceChange;
        ExperienceManager.OnLevelChange -= OnLevelChange;
    }
    
    private static void OnExperienceChange(long experienceGained)
    {
        TotalExperience += experienceGained;
        CurrentExperience += experienceGained;
    }

    private static void OnLevelChange(int levelID)
    {
        Level = levelID + 1;
        CurrentExperience = 0;
    }

    private static void LoadPlayerInformation()
    {
        var playerSaveData = SaveSystem.LoadPlayer();
        
        if (playerSaveData == null) return;

        PlayerID = playerSaveData.playerID;
        TotalExperience = playerSaveData.totalExperience;
        CurrentExperience = playerSaveData.currentExperience;
        Level = playerSaveData.level;
    }
    
    public static void ResetPlayerInformation()
    {
        #if UNITY_EDITOR//This is for testing purposes. We never want to reset the player information once built.
        PlayerID = 0;
        Level = 0;
        TotalExperience = 0;
        CurrentExperience = 0;
        #endif
        Debug.LogWarning("Build is attempting to reset the Player Information class.");
    }
}

[Serializable]
public class PlayerInformationSaveData
{
    public int playerID;
    public int level;
    public long totalExperience;
    public long currentExperience;

    public PlayerInformationSaveData()
    {
        playerID = PlayerInformation.PlayerID;
        level = PlayerInformation.Level;
        totalExperience = PlayerInformation.TotalExperience;
        currentExperience = PlayerInformation.CurrentExperience;
    }
}
