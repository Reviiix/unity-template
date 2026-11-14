using UnityEngine;
using UnityTemplate.Audio;
using UnityTemplate.Core;
using UnityTemplate.Progression;

namespace UnityTemplate.Gameplay
{
    /// <summary>Runs one stage: owns its timer and score, pauses the stage, and submits the high score when the stage ends.</summary>
    public class GameManager : MonoBehaviour
    {
        private const float PausedVolumeFactor = 0.2f;

        public TimeTracker GameTime { get; } = new();
        public ScoreTracker ScoreTracker { get; } = new();
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            GameTime.Start();
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<GameManager>();
        }

        /// <summary>Pauses or resumes the stage timer and lowers the volume while paused.</summary>
        [ContextMenu(nameof(TogglePause))]
        public void TogglePause()
        {
            IsPaused = !IsPaused;
            if (IsPaused) GameTime.Pause();
            else GameTime.Resume();

            if (ServiceLocator.TryGet<AudioManager>(out var audioManager))
            {
                AudioListener.volume = audioManager.IsMuted ? 0 : audioManager.Volume * (IsPaused ? PausedVolumeFactor : 1);
            }
        }

        /// <summary>Adds points to the stage score.</summary>
        public void IncrementScore(int amount)
        {
            ScoreTracker.Increment(amount);
        }

        /// <summary>Stops the timer and records the score as a high score if it beats the previous best.</summary>
        [ContextMenu(nameof(EndGame))]
        public void EndGame()
        {
            GameTime.Pause();
            ServiceLocator.Get<ProgressService>().SubmitScore(ScoreTracker.Score);
        }
    }
}
