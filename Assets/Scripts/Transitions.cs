using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class Transitions
{
    private const bool UseTween = true;

    public static void FadeIn(Graphic imageToFade, float duration, Action callback = null)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (UseTween)
        {
            FadeInWithTween(imageToFade, duration, callback);
            return;
        }
        GameManager.instance.StartCoroutine(FadeInWithCoroutine(imageToFade, duration, callback));
    }
    
    public static void FadeOut(Graphic imageToFade, float duration, Action callback = null)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (UseTween)
        {
            FadeOutWithTween(imageToFade, duration, callback);
            return;
        }
        GameManager.instance.StartCoroutine(FadeOutWithCoroutine(imageToFade, duration, callback));
    }
    
    #region Coroutines
    private static IEnumerator FadeInWithCoroutine(Graphic imageToFade, float slowDown, Action callBack = null)
    {
        var originalColor = imageToFade.color;
            
        while (imageToFade.color.a < 1)
        {
            var alpha = imageToFade.color.a;
            alpha += Time.deltaTime / slowDown;
            imageToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b,alpha);
            yield return null;
        }
        callBack?.Invoke();
    }
    
    private static IEnumerator FadeOutWithCoroutine(Graphic imageToFade, float slowDown, Action callBack = null)
    {
        var originalColor = imageToFade.color;

        while (imageToFade.color.a > 0)
        {
            var alpha = imageToFade.color.a;
            alpha -= Time.deltaTime / slowDown;
            imageToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b,alpha);
            yield return null;
        }

        callBack?.Invoke();
    }
    #endregion Coroutines
    
    #region Tweening
    private static void FadeInWithTween(Graphic imageToFade, float duration, Action callback = null)
    {
        imageToFade.DOFade(1f, duration).SetEase(Ease.OutSine).OnComplete(() => { callback?.Invoke(); });
    }
        
    public static void FadeOutWithTween(Graphic imageToFade, float duration, Action callback = null)
    {
        imageToFade.DOFade(0f, duration).SetEase(Ease.InSine).OnComplete(() => { callback?.Invoke(); });
    }
    #endregion Tweening
}
