using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//The audio manager will Instantiate the maximum amount of audio sources set in the object pool and use the first inactive audio source to play a clip
//This method limits the amount of audio sources in the scene and can have a dramatic perfomance impact as the game scales but means the developer has to anticipate the amount of sounds that can play at one time.
[Serializable]
public class BaseAudioManager : MonoBehaviour
{
    private static readonly HashSet<AudioSource> AudioSources = new HashSet<AudioSource>();
    private static readonly Vector3 AudioSourceStartingLocation = Vector3.zero;
    private const int PoolIndex = 0;
    [SerializeField]
    private AudioClip buttonClick;

    private void Start()
    {
        Initialise();
    }
    
    public void Initialise()
    {
        for (var i = 0; i < GameManager.instance.objectPooling.pools[PoolIndex].maximumActiveObjects; i++)
        {
             var audioSource = ObjectPooling.ReturnObjectFromPool(0, AudioSourceStartingLocation, Quaternion.identity).GetComponent<AudioSource>();
             audioSource.transform.parent = transform;
             AudioSources.Add(audioSource);
        }
    }
    
    public void PlayButtonClickSound()
    {
        PlayClip(buttonClick);
    }

    public static void PlayClip(AudioClip clip, bool looping = false)
    {
        var audioSource = ReturnFirstUnusedAudioSource();
        audioSource.clip = clip;
        audioSource.loop = looping;
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

    public static void SetGlobalVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
