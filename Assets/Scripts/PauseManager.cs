using Statistics;

public static class PauseManager
{
    public static bool isPaused;
    private const float NormalAudioVolume = 1;
    private const float PauseAudioVolume = 0.2f;
    
    public static void PauseButtonPressed()
    {
        isPaused = !isPaused;
        PauseGame(isPaused);
    }

    private static void PauseGame(bool state)
    {
        GameManager.instance.userInterface.EnablePauseCanvas(state);
        EnableGamePlayMethods(!state);
    }

    private static void EnableGamePlayMethods(bool state)
    {
        if (state)
        {
            TimeTracker.ResumeTimer();
            BaseAudioManager.SetGlobalVolume(NormalAudioVolume);
            return;
        }
        TimeTracker.PauseTimer();
        BaseAudioManager.SetGlobalVolume(PauseAudioVolume);
    }
}
