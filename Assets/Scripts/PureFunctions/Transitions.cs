using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PureFunctions
{
    public static class Transitions
    {
        private static MonoBehaviour _coRoutineHandler;

        public static void Initialise(MonoBehaviour coRoutineHandler)
        {
            _coRoutineHandler = coRoutineHandler;
        }

        public static void FadeIn(Graphic imageToFade, float duration, Action callback = null)
        {
            _coRoutineHandler.StartCoroutine(FadeInWithCoroutine(imageToFade, duration, callback));
        }
    
        public static void FadeOut(Graphic imageToFade, float duration, Action callback = null)
        {
            _coRoutineHandler.StartCoroutine(FadeOutWithCoroutine(imageToFade, duration, callback));
        }
        
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
    }
}
