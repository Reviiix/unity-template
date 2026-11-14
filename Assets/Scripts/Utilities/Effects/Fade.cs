using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    /// <summary>
    /// This class will fade graphics in and out by either manipulating the canvas or alpha value of an graphic.
    /// </summary>
    public static class Fade
    {
        public static IEnumerator FadeImageAlphaUp(Graphic imageToFade, float slowDown, Action callBack = null)
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
    
        public static IEnumerator FadeImageAlphaDown(Graphic imageToFade, float slowDown, Action callBack = null)
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
        
        public static IEnumerator FadeCanvasGroupUp(CanvasGroup group, float endValue, float seconds)
        {
            const float timeIncrement = 0.01f;
            var difference = endValue - group.alpha;
            var amountOfLoops = seconds / timeIncrement;
            var valueIncrement = difference / amountOfLoops;
            while (seconds > 0)
            {
                seconds-=timeIncrement;
                group.alpha += valueIncrement;
                yield return null;
            }
            group.alpha = endValue;
        }
        
        public static IEnumerator FadeCanvasGroupDown(CanvasGroup group, float endValue, float seconds)
        {
            const float timeIncrement = 0.01f;
            var difference = endValue - group.alpha;
            var amountOfLoops = seconds / timeIncrement;
            var valueIncrement = difference / amountOfLoops;
            while (seconds > 0)
            {
                seconds-=timeIncrement;
                group.alpha -= valueIncrement;
                yield return null;
            }
            group.alpha = endValue;
        }
    }
}
