using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Accessibility;
using UnityTemplate.Core;

namespace UnityTemplate.Audio
{
    /// <summary>The speaker button and volume slider: mirrors AudioManager's saved state and forwards the player's changes to it.</summary>
    public class VolumeController : MonoBehaviour, IScreenReaderSelection
    {
        [SerializeField] private Image volumeImage;
        [SerializeField] private Sprite[] volumeSprites;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Button muteButton;
        private AudioManager audioManager;

        /// <summary>Wires the speaker button and the slider.</summary>
        private void Awake()
        {
            muteButton.onClick.AddListener(OnMuteButtonPressed);
            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        /// <summary>Shows the saved volume and follows later changes.</summary>
        private void OnEnable()
        {
            audioManager = ServiceLocator.Get<AudioManager>();
            audioManager.VolumeChanged += ShowCurrentVolume;
            ShowCurrentVolume();
        }

        /// <summary>Stops following volume changes.</summary>
        private void OnDisable()
        {
            audioManager.VolumeChanged -= ShowCurrentVolume;
        }

        /// <summary>The mute button reads as selected while the sound is silent.</summary>
        public bool IsSelected(Selectable control)
        {
            return control == muteButton && (audioManager.IsMuted || audioManager.Volume <= 0);
        }

        /// <summary>Toggles mute; unmuting a slider-silenced volume restores full volume.</summary>
        private void OnMuteButtonPressed()
        {
            var silent = audioManager.IsMuted || audioManager.Volume <= 0;
            if (silent && audioManager.Volume <= 0) audioManager.SetVolume(AudioManager.MaximumVolume);
            audioManager.SetMuted(!silent);
        }

        /// <summary>Unmutes and sets the volume from the slider.</summary>
        private void OnSliderValueChanged(float value)
        {
            audioManager.SetMuted(false);
            audioManager.SetVolume(value);
        }

        /// <summary>Moves the slider and picks the speaker icon for the audible volume.</summary>
        private void ShowCurrentVolume()
        {
            var audibleVolume = audioManager.IsMuted ? 0 : audioManager.Volume;
            volumeSlider.SetValueWithoutNotify(audibleVolume);
            volumeImage.sprite = volumeSprites[GetSpriteIndex(audibleVolume)];
        }

        /// <summary>Picks the speaker icon for a volume: the first icon is silent, the last is loudest.</summary>
        private int GetSpriteIndex(float volume)
        {
            var step = 1f / volumeSprites.Length;
            for (var i = 0; i < volumeSprites.Length; i++)
            {
                if (volume <= step * i) return i;
            }
            return volumeSprites.Length - 1;
        }
    }
}
