using Audio;

public static class PauseManager
{
    public static bool IsPaused
    {
        get;
        private set;
    }

    private const float NormalAudioVolume = AudioManager.VolumeControls.MaximumVolume;
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
            GameManager.Instance.GameTime.ResumeTimer();
            BaseAudioManager.SetGlobalVolumeForPause(NormalAudioVolume);
            return;
        }
        GameManager.Instance.GameTime.PauseTimer();
        BaseAudioManager.SetGlobalVolumeForPause(PauseAudioVolume);
    }
}
