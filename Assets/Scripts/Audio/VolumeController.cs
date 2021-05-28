using System;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    public class VolumeController : MonoBehaviour
    {
        public static Action<bool, float> OnMuteButtonPressed;
        public static Action<float> OnSliderValueChanged;
        [SerializeField] private Image volumeImage;
        [SerializeField] private Sprite[] volumeSprites;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Button muteButton;
        private bool _muteButtonState;
        private Sprite _nonMuteSpriteCache;
        private float _nonMuteVolumeCache;

        private void OnEnable()
        {
            SaveSystem.OnSaveDataLoaded += Load;
            SetButtonEvents();
        }

        private void OnDisable()
        {
            SaveSystem.OnSaveDataLoaded -= Load;
        }

        private void Load(SaveSystem.SaveData saveData)
        {
            MatchVolumeSliderWithIcon(saveData.Volume);
            MatchVolumeIconWithSlider(saveData.Volume);
        }
        
        private void SetButtonEvents()
        {
            muteButton.onClick.AddListener(MuteButtonPressed);
            volumeSlider.onValueChanged.AddListener(SliderValueChanged);
        }

        private void MuteButtonPressed()
        {
            var volume = 0f;
            _muteButtonState = !_muteButtonState;
            if (_muteButtonState)
            {
                _nonMuteSpriteCache = volumeImage.sprite;
                _nonMuteVolumeCache = BaseAudioManager.CurrentVolume;
                volumeImage.sprite = volumeSprites[(int)volume];
                MatchVolumeSliderWithIcon(volume);
            }
            else
            {
                volume = _nonMuteVolumeCache;
                volumeImage.sprite = _nonMuteSpriteCache;
                MatchVolumeSliderWithIcon(volume);
            }
            OnMuteButtonPressed?.Invoke(_muteButtonState, volume);
        }

        private void SliderValueChanged(float value)
        {
            _muteButtonState = value == 0;
            MatchVolumeIconWithSlider(value);
            OnSliderValueChanged?.Invoke(value);
        }

        private void MatchVolumeIconWithSlider(float volume)
        {
            volumeImage.sprite = volumeSprites[ReturnVolumeSpriteIndex(volume)];
        }
        
        private void MatchVolumeSliderWithIcon(float volume)
        {
            volumeSlider.value = volume;
        }

        private int ReturnVolumeSpriteIndex(float volume)
        {
            float increments = 1;
            var amountOfSprites = volumeSprites.Length;
            increments /= amountOfSprites;
            for (var i = 0; i < amountOfSprites; i++)
            {
                if (volume <= increments * i) return i;
            }
            return amountOfSprites-1;
        }
    }
}