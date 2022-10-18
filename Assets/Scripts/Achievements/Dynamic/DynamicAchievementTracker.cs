using System;
using UnityEngine;

public class DynamicAchievementTracker : AchievementTracker
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            DynamicAchievementManager.UnlockAchievement(DynamicAchievementManager.Achievement.TotalPlayTimeInSeconds);
        }
    }
    
    protected override void PerformChecks()
    {
        
    }
}
