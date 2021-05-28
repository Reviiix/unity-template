using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PureFunctions
{
    public static class Fade
    {
        public static IEnumerator FadeIn(Graphic imageToFade, float slowDown, Action callBack = null)
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
    
        public static IEnumerator FadeOut(Graphic imageToFade, float slowDown, Action callBack = null)
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
    }
}
