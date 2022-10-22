using System;
using System.Collections;
using PureFunctions.Effects;
using UnityEngine;

namespace Achievements.Display.PopUp
{
    /// <summary>
    /// This class is the base for achievement items (pop ups and items in a list) it is shared by dynamic and permanent achievements.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class AchievementPopUpItem : AchievementItemBase
    {
        private static readonly WaitForSeconds DisplayTime = new WaitForSeconds(3);
        private const float FadeTime = 1;
        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(Sprite iconContent, string titleContent, string descriptionContent, int rewardContent, Action callBack)
        {
            graphic.sprite = iconContent;
            graphic.color = Color.white;
            title.text = titleContent;
            description.text = descriptionContent;
            reward.text = rewardContent.ToString();
            _canvasGroup.alpha = 1;
            gameObject.SetActive(true);
            
            StartCoroutine(ShowThenHide(callBack));
        }

        private IEnumerator ShowThenHide(Action callBack)
        {
            //yield return StartCoroutine(Fade.FadeCanvasGroupUp(_canvasGroup, 1, FadeTime));
            
            yield return DisplayTime;
            
            yield return StartCoroutine(Fade.FadeCanvasGroupDown(_canvasGroup, 0, FadeTime));

            callBack();
        }
    }
}