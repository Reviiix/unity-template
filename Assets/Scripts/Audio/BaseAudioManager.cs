using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

//The audio manager will instantiate the maximum amount of audio sources set in the object pool on start up.
//When playing a clip it will use the first inactive audio source from the hashset.
//This system limits the amount of audio sources in the scene and can have a significant performance improvement as the game scales.
//A drawback of this system is that the developer has to anticipate the amount of sounds that can play at one time.
namespace Audio
{
    [Serializable]
    public class BaseAudioManager : MonoBehaviour
    {
        private static BaseAudioManager _instance;
        private static readonly HashSet<AudioSource> AudioSources = new HashSet<AudioSource>();
        private static readonly Vector3 AudioSourceStartingLocation = Vector3.zero;
        private const int PoolIndex = 0;
        [SerializeField]
        private AudioClip buttonClick;
        [SerializeField]
        private AudioClip menuMovement;
        public VolumeControls volumeControls;

        public void Initialise()
        {
            ResolveDependencies();
            volumeControls.Initialise();
        }

        private void ResolveDependencies()
        {
            _instance = this;
            
            for (var i = 0; i < ProjectManager.Instance.globalObjectPools.pools[PoolIndex].maximumActiveObjects; i++)
            {
                var audioSource = ObjectPooling.ReturnObjectFromPool(0, AudioSourceStartingLocation, Quaternion.identity).GetComponent<AudioSource>();
                audioSource.transform.parent = transform;
                AudioSources.Add(audioSource);
            }
        }

        public static void PlayButtonClickSound()
        {
            PlayClip(_instance.buttonClick);
        }
        
        public static void PlayMenuMovementSound()
        {
            PlayClip(_instance.menuMovement);
        }

        protected static void PlayClip(AudioClip clip)
        {
            var audioSource = ReturnFirstUnusedAudioSource();
            audioSource.clip = clip;
            audioSource.Play();
        }

        private static AudioSource ReturnFirstUnusedAudioSource()
        {
            foreach (var audioSource in AudioSources)
            {
                if (audioSource.isPlaying)
                {
                    continue;
                }
                return audioSource;
            }
            Debug.LogWarning("There are not enough audio sources to play that many sounds at once, please set a higher maximum amount in the object pool. The first or default audio source has been returned and may have cut of sounds unexpectedly.");
            return AudioSources.FirstOrDefault();
        }

        public static void SetGlobalVolumeForPause(float volume)
        {
            //This method is only to be used for manipulating the volume in a pause screen (So volume controls dont show pause volume).
            //To manipulate the volume do it through the volume controls class to ensure all appropriate actions are taken.
            AudioListener.volume = volume;
        }
    }

    [Serializable]
    public class VolumeControls
    {
        #region Volume
        private static readonly float[] VolumeLevels = {MinimumVolume, 0.3f, 0.6f, MaximumVolume};
        [Range(0,1)]
        private const float MinimumVolume = 0;
        [Range(0,1)]
        private const float MaximumVolume = 1;
        private static float _volumeLevel = MaximumVolume;
        public static float VolumeLevel
        {
            get => _volumeLevel;
            private set
            {
                if (value > MaximumVolume)
                {
                    _volumeLevel = MinimumVolume;
                    return;
                }
                if (value < MinimumVolume)
                {
                    _volumeLevel = MaximumVolume;
                    return;
                }

                _volumeLevel = value;
            }
        }
        #endregion Volume
        [SerializeField]
        private Sprite[] volumeStages;
        [SerializeField]
        private Slider volumeSlider;
        [SerializeField]
        private Button volumeButton;
        private Image _volumeImage;

        public void Initialise()
        {
            ResolveDependencies();
            AddButtonEvents();
        }
        
        private void ResolveDependencies()
        {
            _volumeImage = volumeButton.GetComponent<Image>();
        }

        private void AddButtonEvents()
        {
            volumeSlider.onValueChanged.AddListener(SetGlobalVolume);
            volumeButton.onClick.AddListener(IncrementVolume);
        }

        private void MatchSliderAndGraphic()
        {
            SetVolumeSlider();
            SetVolumeGraphic();
        }

        private void SetVolumeGraphic()
        {
            _volumeImage.sprite = volumeStages[ReturnVolumeLevelAsIndex(VolumeLevel)];
        }
        
        private void SetVolumeSlider()
        {
            volumeSlider.value = VolumeLevel;
        }

        private void IncrementVolume()
        {
            for (var i = VolumeLevels.Length - 1; i >= 0; i--)
            {
                if (VolumeLevel < VolumeLevels[i]) continue;
                
                var index = i+1;
                if (i == VolumeLevels.Length-1)
                {
                    index = 0;
                }
                    
                VolumeLevel = VolumeLevels[index];
                break;
            }
            SetGlobalVolume(VolumeLevel);
        }

        private void SetGlobalVolume(float newVolume)
        {
            VolumeLevel = newVolume;
            AudioListener.volume = VolumeLevel;
            MatchSliderAndGraphic();
        }
        
        private static int ReturnVolumeLevelAsIndex(float volumeLevel)
        {
            if (volumeLevel >= VolumeLevels[2])
            {
                return 3;
            }
            
            if (volumeLevel >= VolumeLevels[1])
            {
                return 2;
            }
            
            if (volumeLevel > MinimumVolume)
            {
                return 1;
            }
            
            return 0;
        }
    }
}
