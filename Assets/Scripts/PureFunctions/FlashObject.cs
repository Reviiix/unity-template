using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PureFunctions
{
    public static class FlashObject
    {
        private const float FlashTime = 0.1f;
        private static readonly WaitForSeconds WaitFlashTime = new WaitForSeconds(FlashTime);

        public static IEnumerator FlashGraphic(Graphic renderer, float cycles, Action completionCallBack = null)
        {
            for (var i = 0; i < cycles; i++)
            {
                renderer.enabled = !renderer.enabled;
                yield return WaitFlashTime;
            }
            renderer.enabled = true;
            completionCallBack?.Invoke();
        }
        
        public static IEnumerator FlashGameObject(GameObject objectToFlash, float cycles, Action completionCallBack = null)
        {
            for (var i = 0; i < cycles; i++)
            {
                objectToFlash.SetActive(!objectToFlash.activeInHierarchy);
                yield return WaitFlashTime;
            }
            objectToFlash.SetActive(true);
            completionCallBack?.Invoke();
        }
    }
}
