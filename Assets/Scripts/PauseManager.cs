using Audio;
using Statistics;

public static class PauseManager
{
    public static bool IsPaused
    {
        get;
        private set;
    }
    private static float NormalAudioVolume => ProjectSettings.CurrentVolume;
    private const float PauseAudioVolume = 0.2f;
    
    public static void PauseButtonPressed()
    {
        IsPaused = !IsPaused;
        PauseGame(IsPaused);
    }

    private static void PauseGame(bool state = true)
    {
        EnableGamePlayMethods(!state);
    }

    private static void EnableGamePlayMethods(bool state = true)
    {
        if (state)
        {
            TimeTracker.ResumeTimer();
            BaseAudioManager.SetGlobalVolumeForPause(NormalAudioVolume);
            return;
        }
        TimeTracker.PauseTimer();
        BaseAudioManager.SetGlobalVolumeForPause(PauseAudioVolume);
    }
}
