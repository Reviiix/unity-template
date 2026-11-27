using DG.Tweening;
using TMPro;
using UnityEngine;

namespace UnityTemplate.UserInterface
{
    /// <summary>A short message at the bottom of the screen that fades in, stays for a few seconds and fades out; taps pass through it.</summary>
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup))]
    public class NoticeToast : MonoBehaviour
    {
        private const float FadeSeconds = 0.25f;
        [SerializeField] private TMP_Text message;
        private Canvas canvas;
        private CanvasGroup group;
        private Sequence showing;

        public bool IsShowing => canvas.enabled;

        /// <summary>Starts hidden and never blocks taps.</summary>
        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            group = GetComponent<CanvasGroup>();
            group.alpha = 0;
            group.blocksRaycasts = false;
            group.interactable = false;
            canvas.enabled = false;
        }

        /// <summary>Shows the text for the given time (plus a short fade in and out), replacing any message already showing.</summary>
        public void Show(string text, float seconds)
        {
            message.SetText(text);
            showing?.Kill();
            canvas.enabled = true;
            showing = DOTween.Sequence()
                .Append(DOTween.To(() => group.alpha, SetAlpha, 1, FadeSeconds))
                .AppendInterval(seconds)
                .Append(DOTween.To(() => group.alpha, SetAlpha, 0, FadeSeconds))
                .OnComplete(() => canvas.enabled = false)
                .SetUpdate(true)
                .SetLink(gameObject);
        }

        /// <summary>Sets the toast's opacity.</summary>
        private void SetAlpha(float alpha)
        {
            group.alpha = alpha;
        }
    }
}
