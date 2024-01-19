using System.Collections.Generic;
using PureFunctions.UnitySpecific;
using UnityEngine;
using UnityEngine.UI;

namespace Audio
{
    /// <summary>
    /// This class is the base for audio managers which will manage what sounds play and when
    /// The audio manager will instantiate the maximum amount of audio sources set in the object pool on start up.
    /// When playing a clip it will use the first inactive audio source from the hashset.
    /// This system limits the amount of audio sources in the scene and can have a significant performance improvement as the game scales.
    /// A drawback of this system is that the developer has to anticipate the amount of sounds that can play at one time.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public abstract class BaseAudioManager : PrivateSingleton<BaseAudioManager>
    {
        private static AudioSource _backGroundAudioSource;
        private static readonly Queue<AudioSource> AudioSources = new Queue<AudioSource>();
        private const int PoolIndex = 0;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip menuMovement;
        private readonly VolumeControls volumeControls = new VolumeControls();
        public static float CurrentVolume => Instance.volumeControls.VolumeLevel;

        public void Initialise()
        {
            volumeControls.Initialise();
            ResolveDependencies();
            AssignAllButtonsTheClickClickSound();
        }

        private void ResolveDependencies()
        {
            var maximumNumberOfAudioSources = ObjectPooler.GetMaximumActiveObjects(PoolIndex);

            _backGroundAudioSource = GetComponent<AudioSource>();
            
            for (var i = 0; i < maximumNumberOfAudioSources; i++)
            {
                var audioSource = ObjectPooler.GetObjectFromPool(0, Vector3.zero, Quaternion.identity).GetComponent<AudioSource>();
                audioSource.transform.parent = transform;
                AudioSources.Enqueue(audioSource);
            }
        }

        private static void AssignAllButtonsTheClickClickSound()
        {
            var allButtons = FindObjectsOfType<Button>();

            foreach (var button in allButtons)
            {
                button.onClick.RemoveListener(PlayButtonClickSound);
                
                button.onClick.AddListener(PlayButtonClickSound);
            }
        }

        private static void PlayButtonClickSound()
        {
            PlayClip(Instance.buttonClick);
        }
        
        public static void PlayMenuMovementSound()
        {
            PlayClip(Instance.menuMovement);
        }

        protected static void PlayClip(AudioClip clip)
        {
            var audioSource = ReturnFirstUnusedAudioSource();
            audioSource.clip = clip;
            audioSource.Play();
        }

        private static AudioSource ReturnFirstUnusedAudioSource()
        {
            var audioSource = AudioSources.Dequeue();
            AudioSources.Enqueue(audioSource);
            return audioSource;
        }
    

        public static void SetGlobalVolumeForPause(float volume)
        {
            AudioListener.volume = volume;
        }
        
        public class VolumeControls
        {
            private const float MinimumVolume = 0;
            public const float MaximumVolume = 1;
            private float _volumeLevel = MaximumVolume;
            public float VolumeLevel
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

            private void SetGlobalVolume(float newVolume)
            {
                VolumeLevel = newVolume;
                AudioListener.volume = VolumeLevel;
            }

            public void Initialise()
            {
                SaveSystem.OnSaveDataLoaded += OnSaveDataLoaded;
                VolumeController.OnMuteButtonPressed += OnMuteButtonPressed;
                VolumeController.OnSliderValueChanged += OnSliderValueChanged;
            }

            private void OnSaveDataLoaded(SaveSystem.SaveData saveData)
            {
                if (saveData == null) return;
                
                VolumeLevel = saveData.volume;
            }

            private void OnMuteButtonPressed(bool state, float volume)
            {
                switch (state)
                {
                    case true:
                        SetGlobalVolume(0);
                        return;
                    case false:
                        SetGlobalVolume(volume);
                        return;
                }
            }
            
            private void OnSliderValueChanged(float value)
            {
                SetGlobalVolume(value);
            }
        }
    }
}
