using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Theming;

namespace UnityTemplate.UserInterface.Kit
{
    /// <summary>The kit's on/off switch: a Toggle whose knob slides along a pill track that fills with the accent when on.</summary>
    [RequireComponent(typeof(Toggle))]
    public class ToggleSwitch : MonoBehaviour
    {
        private const float SlideSeconds = 0.15f;
        [SerializeField] private Image track;
        [SerializeField] private RectTransform knob;
        [SerializeField] private float knobTravel = 20f;
        [SerializeField] private ThemeColour onColour = ThemeColour.Accent;
        [SerializeField] private ThemeColour offColour = ThemeColour.Track;
        private Toggle toggle;
        private ThemeService theme;

        public Toggle Toggle => toggle != null ? toggle : toggle = GetComponent<Toggle>();

        /// <summary>Caches the Toggle and slides the knob whenever the player flips it.</summary>
        private void Awake()
        {
            Toggle.onValueChanged.AddListener(_ => ShowState(true));
        }

        /// <summary>Shows the current state and recolours the track when the theme changes.</summary>
        private void OnEnable()
        {
            if (ServiceLocator.TryGet(out theme)) theme.Changed += ColourTrackNow;
            ShowState(false);
        }

        /// <summary>Stops following theme changes and finishes any slide.</summary>
        private void OnDisable()
        {
            if (theme != null) theme.Changed -= ColourTrackNow;
            knob.DOKill(true);
            track.DOKill(true);
        }

        /// <summary>Sets the switch without raising onValueChanged, e.g. to mirror a saved setting.</summary>
        public void SetIsOnWithoutNotify(bool isOn)
        {
            Toggle.SetIsOnWithoutNotify(isOn);
            ShowState(false);
        }

        /// <summary>Moves the knob to the on or off end and colours the track, sliding when animate is true.</summary>
        private void ShowState(bool animate)
        {
            var x = Toggle.isOn ? knobTravel : -knobTravel;
            knob.DOKill();
            if (animate && isActiveAndEnabled) DOTween.To(() => knob.anchoredPosition.x, value => knob.anchoredPosition = new Vector2(value, knob.anchoredPosition.y), x, SlideSeconds).SetEase(Ease.OutQuad).SetTarget(knob).SetLink(gameObject);
            else knob.anchoredPosition = new Vector2(x, knob.anchoredPosition.y);
            ColourTrack(animate);
        }

        /// <summary>Recolours the track at once for the new theme.</summary>
        private void ColourTrackNow()
        {
            ColourTrack(false);
        }

        /// <summary>Colours the track for the current state and theme, fading when animate is true.</summary>
        private void ColourTrack(bool animate)
        {
            if (theme == null) return;
            var colour = theme.Get(Toggle.isOn ? onColour : offColour);
            track.DOKill();
            if (animate && isActiveAndEnabled) DOTween.To(() => track.color, value => track.color = value, colour, SlideSeconds).SetTarget(track).SetLink(gameObject);
            else track.color = colour;
        }
    }
}
