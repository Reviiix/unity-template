using System;
using UnityEngine;
using UnityEngine.UI;
using UnityTemplate.Core;

namespace UnityTemplate.Audio
{
    /// <summary>Plays UI sounds through a fixed set of pooled AudioSources and owns the saved master volume and mute state.</summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour, ISaveable
    {
        public const float MaximumVolume = 1;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip menuMovement;
        private AudioSource[] audioSources = Array.Empty<AudioSource>();
        private int nextAudioSource;

        public float Volume { get; private set; } = MaximumVolume;
        public bool IsMuted { get; private set; }
        public event Action VolumeChanged;

        /// <summary>Takes the pooled AudioSources and gives every button in the scene its click sound; called once by GameInitialiser.</summary>
        public void Initialise()
        {
            var pool = ServiceLocator.Get<ObjectPooler>();
            var amountOfSources = pool.GetPoolSize(PoolKey.AudioSource);
            audioSources = new AudioSource[amountOfSources];
            for (var i = 0; i < amountOfSources; i++)
            {
                var source = pool.Get(PoolKey.AudioSource);
                source.transform.SetParent(transform, false);
                audioSources[i] = source.GetComponent<AudioSource>();
            }

            foreach (var button in FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                button.onClick.RemoveListener(PlayButtonClickSound);
                button.onClick.AddListener(PlayButtonClickSound);
            }
        }

        /// <summary>Plays the whoosh used when menus and popups move.</summary>
        public void PlayMenuMovementSound()
        {
            PlayClip(menuMovement);
        }

        /// <summary>Sets the master volume (0-1); it is saved with the next save.</summary>
        public void SetVolume(float volume)
        {
            Volume = Mathf.Clamp01(volume);
            ApplyVolume();
        }

        /// <summary>Mutes or unmutes all sound without forgetting the chosen volume.</summary>
        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            ApplyVolume();
        }

        /// <summary>Restores volume and mute state from the save file and applies them immediately.</summary>
        public void Load(SaveData data)
        {
            Volume = Mathf.Clamp01(data.volume);
            IsMuted = data.muted;
            AudioListener.volume = IsMuted ? 0 : Volume;
            VolumeChanged?.Invoke();
        }

        /// <summary>Writes volume and mute state to the save file.</summary>
        public void Save(SaveData data)
        {
            data.volume = Volume;
            data.muted = IsMuted;
        }

        private void PlayButtonClickSound()
        {
            PlayClip(buttonClick);
        }

        private void PlayClip(AudioClip clip)
        {
            if (audioSources.Length == 0 || clip == null) return;

            var source = audioSources[nextAudioSource];
            nextAudioSource = (nextAudioSource + 1) % audioSources.Length;
            source.clip = clip;
            source.Play();
        }

        private void ApplyVolume()
        {
            AudioListener.volume = IsMuted ? 0 : Volume;
            VolumeChanged?.Invoke();
        }
    }
}
