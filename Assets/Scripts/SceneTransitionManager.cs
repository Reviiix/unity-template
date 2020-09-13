using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class SceneTransitionManager 
{
    private enum SceneIndices
    {
        Initialisation = 0,
        MainGame = 1
    }
    private static readonly RawImage FadeImage = UserInterfaceManager.transitionalFadeImage;
    private const int Duration = 1;
        
    public static void LoadMainScene(Action sequenceCompleteCallBack)
    {
        FadeInAndOut(FadeImage, Duration, ()=>
        {
            SceneManager.LoadScene((int)SceneIndices.MainGame);
        }, sequenceCompleteCallBack);
    }

    private static void FadeInAndOut(Graphic imageToFade, float duration, Action fadeInCallBack = null, Action fadeOutCallBack = null)
    {
        Transitions.FadeIn(imageToFade, duration, () =>
        {
            fadeInCallBack?.Invoke();
            Transitions.FadeOut(imageToFade, duration, fadeOutCallBack);
        });
    }
}
