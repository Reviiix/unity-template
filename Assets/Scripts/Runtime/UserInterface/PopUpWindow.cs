using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityTemplate.Audio;
using UnityTemplate.Core;

namespace UnityTemplate.UserInterface
{
    /// <summary>Base class for popups: slides the panel in from the left and out again, and ignores taps while a slide is running.</summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class PopUpWindow : MonoBehaviour
    {
        private const float SlideDuration = 0.87f;
        private const int OffScreenMargin = 500;
        [SerializeField] protected Transform popUpMenu;
        [FormerlySerializedAs("closeMenu")] [SerializeField] protected Button closeButton;
        private Canvas canvas;
        private CanvasGroup contentGroup;
        private Tween slide;

        public PopUpState State { get; private set; } = PopUpState.Hidden;
        public event Action<PopUpWindow> Closed;

        protected Canvas Canvas => canvas;

        /// <summary>Caches the canvas and the panel's canvas group, wires X and starts hidden.</summary>
        protected virtual void Awake()
        {
            canvas = GetComponent<Canvas>();
            contentGroup = popUpMenu.GetComponent<CanvasGroup>();
            if (contentGroup == null) contentGroup = popUpMenu.gameObject.AddComponent<CanvasGroup>();
            if (closeButton != null) closeButton.onClick.AddListener(OnCloseButtonPressed);
            canvas.enabled = false;
        }

        /// <summary>Stops the slide before the popup is destroyed.</summary>
        protected virtual void OnDestroy()
        {
            slide?.Kill();
        }

        /// <summary>Shows the popup with its slide-in animation.</summary>
        public void Open()
        {
            if (State != PopUpState.Hidden) return;

            State = PopUpState.Opening;
            OnOpening();
            canvas.enabled = true;
            SetContentInteractable(true);
            popUpMenu.localPosition = new Vector3(-OffScreenDistance, 0, 0);
            Slide(Vector3.zero, () => State = PopUpState.Open);
            PlayMenuSound();
        }

        /// <summary>Hides the popup at once (no visible slide) and finishes closing once the hidden slide completes.</summary>
        public void CloseImmediately()
        {
            if (State is PopUpState.Hidden or PopUpState.Closing) return;

            BeginClosing();
            canvas.enabled = false;
            PlayMenuSound();
        }

        /// <summary>Slides the popup out to the left, then finishes closing.</summary>
        protected void CloseWithSlide(bool playSound = false)
        {
            if (State is PopUpState.Hidden or PopUpState.Closing) return;

            BeginClosing();
            if (playSound) PlayMenuSound();
        }

        /// <summary>Lets the panel's buttons receive taps or not; the dimmed backdrop keeps blocking the menus underneath either way.</summary>
        protected void SetContentInteractable(bool interactable)
        {
            contentGroup.blocksRaycasts = interactable;
        }

        /// <summary>Handles the Android back button while this popup is the top one.</summary>
        public void PressBack()
        {
            if (State == PopUpState.Open) OnBackPressed();
        }

        /// <summary>Called just before the slide-in starts; popups override it to reset their contents.</summary>
        protected virtual void OnOpening()
        {
        }

        /// <summary>Called by the close (X) button; popups override it to add behaviour such as claiming a reward.</summary>
        protected virtual void OnCloseButtonPressed()
        {
            CloseWithSlide();
        }

        /// <summary>Called for the Android back button; by default it acts like the close button, if the popup has one.</summary>
        protected virtual void OnBackPressed()
        {
            if (closeButton != null && closeButton.isActiveAndEnabled) OnCloseButtonPressed();
        }

        /// <summary>Called after the closing slide; the default releases the popup instance.</summary>
        protected virtual void OnClosed()
        {
            ServiceLocator.Get<UserInterfaceManager>().ReleasePopUp(gameObject);
        }

        /// <summary>Blocks the panel's buttons and slides it off-screen, then finishes closing.</summary>
        private void BeginClosing()
        {
            State = PopUpState.Closing;
            SetContentInteractable(false);
            Slide(new Vector3(-OffScreenDistance, 0, 0), () =>
            {
                State = PopUpState.Hidden;
                canvas.enabled = false;
                Closed?.Invoke(this);
                OnClosed();
            });
        }

        /// <summary>Moves the panel with the original ease-out curve.</summary>
        private void Slide(Vector3 target, Action onComplete)
        {
            slide?.Kill();
            slide = popUpMenu.DOLocalMove(target, SlideDuration).SetEase(Ease.OutExpo).SetLink(gameObject).OnComplete(() => onComplete());
        }

        private static float OffScreenDistance => Screen.width * 2 + OffScreenMargin;

        /// <summary>Plays the menu whoosh if audio is available.</summary>
        private static void PlayMenuSound()
        {
            if (ServiceLocator.TryGet<AudioManager>(out var audioManager)) audioManager.PlayMenuMovementSound();
        }
    }

    /// <summary>The stages of a popup's lifetime; input is only accepted while Open.</summary>
    public enum PopUpState
    {
        Hidden,
        Opening,
        Open,
        Closing,
    }
}
