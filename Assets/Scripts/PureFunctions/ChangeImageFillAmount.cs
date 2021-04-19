using System;
using System.Collections;
using UnityEngine.UI;

namespace PureFunctions
{
    public static class ChangeImageFillAmount 
    {
        public static IEnumerator FillImage(Image levelBar, float experienceRepresentedAsBarValue, float fillIncrement = 0.01f, Action callBack = null)
        {
            while (levelBar.fillAmount < experienceRepresentedAsBarValue)
            {
                levelBar.fillAmount += fillIncrement;
                yield return null;
            }
            callBack?.Invoke();
        }
    }
}
