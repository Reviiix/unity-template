using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.UserInterface;

namespace UnityTemplate.Utilities
{
    /// <summary>Opens a web page when the button on the same GameObject is tapped; in WebGL it opens from the browser's own click so pop-up blockers allow it and shows a notice that browsers may block it; a button with no URL hides itself.</summary>
    [RequireComponent(typeof(Button))]
    public class OpenURLButton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler
    {
        [SerializeField] private string url;
        private const string PopUpNotice = "Opening the link in a new tab. Most browsers block pop-ups, so if nothing opens, allow pop-ups for this site.";
        private const float PopUpNoticeSeconds = 5;

        [Tooltip("Hide the button while no URL is set. Turn off for buttons that also show content, like the version text.")]
        [SerializeField] private bool hideWithoutUrl = true;
        private Button button;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern void OpenUrlOnNextClick(string url);
        [DllImport("__Internal")] private static extern void OpenUrlIfStillPending();
        [DllImport("__Internal")] private static extern void CancelOpenUrlOnNextClick();
#endif

        private bool HasUrl => !string.IsNullOrWhiteSpace(url);

        /// <summary>Hides the button if it has no URL and should hide, otherwise wires it to open the URL.</summary>
        private void Awake()
        {
            button = GetComponent<Button>();
            if (!HasUrl)
            {
                if (hideWithoutUrl) gameObject.SetActive(false);
                return;
            }
#if !(UNITY_WEBGL && !UNITY_EDITOR)
            button.onClick.AddListener(() => Application.OpenURL(url));
#endif
        }

        /// <summary>In WebGL, arms the page to open the URL on the click this press ends with (browsers only allow new tabs from a click).</summary>
        public void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (HasUrl && button.IsInteractable()) OpenUrlOnNextClick(url);
#endif
        }

        /// <summary>In WebGL, opens the link now if the browser's click passed before the press was armed (slow frames), and explains that browsers may block the new tab.</summary>
        public void OnPointerClick(PointerEventData eventData)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            if (!HasUrl || !button.IsInteractable()) return;
            OpenUrlIfStillPending();
            if (ServiceLocator.TryGet<UserInterfaceManager>(out var userInterface)) userInterface.ShowNotice(PopUpNotice, PopUpNoticeSeconds);
#endif
        }

        /// <summary>In WebGL, disarms the link when the press slides off the button.</summary>
        public void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            CancelOpenUrlOnNextClick();
#endif
        }
    }
}
