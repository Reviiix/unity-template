using System;
using PureFunctions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserInterface;

public static class SceneTransitionManager 
{
    private enum SceneIndices
    {
        Initialisation = 0,
        Game = 1
    }
    private static readonly RawImage FadeImage = UserInterfaceManager.TransitionalFadeImage;
    #region Fade Duration
    private const int TotalFadeDuration = 3;
    private const int FadeDuration = TotalFadeDuration / 2;
    #endregion

    public static void LoadGameScene(Action sequenceCompleteCallBack)
    {
        FadeInAndOut(FadeImage, FadeDuration, ()=>
        {
            SceneManager.LoadScene((int)SceneIndices.Game);
        }, sequenceCompleteCallBack);
    }
    
    public static void LoadInitialisationScene(Action sequenceCompleteCallBack)
    {
        FadeInAndOut(FadeImage, FadeDuration, ()=>
        {
            SceneManager.LoadScene((int)SceneIndices.Initialisation);
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
