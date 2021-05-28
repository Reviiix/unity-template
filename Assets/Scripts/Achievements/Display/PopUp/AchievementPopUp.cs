using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Achievements.Display.PopUp
{
    [RequireComponent(typeof(GameObject))]
    public class AchievementPopUp : MonoBehaviour
    {
        private static readonly WaitForSeconds DisplayTime = new WaitForSeconds(3);
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text title;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text reward;
        [SerializeField] private CanvasGroup canvasGroup;

        public void Show(Sprite iconContent, string titleContent, string descriptionContent, int rewardContent, Action callBack)
        {
            icon.sprite = iconContent;
            title.text = titleContent;
            description.text = descriptionContent;
            reward.text = rewardContent.ToString();
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            
            StartCoroutine(ShowThenHide(callBack));
        }

        private IEnumerator ShowThenHide(Action callBack)
        {
            var group = canvasGroup;
            
            yield return StartCoroutine(FadeCanvasGroupDown(group, 1));
            
            yield return DisplayTime;
            
            yield return StartCoroutine(FadeCanvasGroupDown(group, 0));

            callBack();
        }

        private static IEnumerator FadeCanvasGroupUp(CanvasGroup group, float endValue)
        {
            //TODO //Extract pure function and allow for time input
            const float increment = 0.01f;
            while (group.alpha < endValue)
            {
                group.alpha += increment;
                yield return null;
            }
            group.alpha = endValue;
        }
        
        private static IEnumerator FadeCanvasGroupDown(CanvasGroup group, float endValue)
        {
            const float increment = 0.01f;
            while (group.alpha > endValue)
            {
                group.alpha -= increment;
                yield return null;
            }
            group.alpha = endValue;
        }
    }
}