using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;

namespace UnityTemplate.Audio
{
    /// <summary>The speaker button and volume slider: mirrors AudioManager's saved state and forwards the player's changes to it.</summary>
    public class VolumeController : MonoBehaviour
    {
        [SerializeField] private Image volumeImage;
        [SerializeField] private Sprite[] volumeSprites;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Button muteButton;
        private AudioManager audioManager;

        private void Awake()
        {
            muteButton.onClick.AddListener(OnMuteButtonPressed);
            volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void OnEnable()
        {
            audioManager = ServiceLocator.Get<AudioManager>();
            audioManager.VolumeChanged += ShowCurrentVolume;
            ShowCurrentVolume();
        }

        private void OnDisable()
        {
            audioManager.VolumeChanged -= ShowCurrentVolume;
        }

        private void OnMuteButtonPressed()
        {
            var silent = audioManager.IsMuted || audioManager.Volume <= 0;
            if (silent && audioManager.Volume <= 0) audioManager.SetVolume(AudioManager.MaximumVolume);
            audioManager.SetMuted(!silent);
        }

        private void OnSliderValueChanged(float value)
        {
            audioManager.SetMuted(false);
            audioManager.SetVolume(value);
        }

        private void ShowCurrentVolume()
        {
            var audibleVolume = audioManager.IsMuted ? 0 : audioManager.Volume;
            volumeSlider.SetValueWithoutNotify(audibleVolume);
            volumeImage.sprite = volumeSprites[GetSpriteIndex(audibleVolume)];
        }

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
