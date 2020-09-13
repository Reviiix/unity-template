using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Statistics
{
    public static class TimeTracker
    {
        private const string TimeDisplayPrefix = "TIME: ";
        private static bool _trackTime;
        private static TMP_Text _timeText;
        private static Coroutine _timeTracker;
        private static DateTime _gameStartTime;
        private static DateTime _pauseStartTime;
        private static DateTime _pauseEndTime;
        private static TimeSpan _timePaused;

        public static void Initialise()
        {
            _timeText = GameManager.instance.userInterface.ReturnTimeText();
            Reset();
        }

        private static void Reset()
        {
            UpdateTimeDisplay(_timeText, DateTime.Now); //_timeText.text = TimeDisplayPrefix + "0:00.00";
            StopTimer();

            _gameStartTime = DateTime.Now;
            _pauseStartTime = DateTime.Now;
            _pauseEndTime = DateTime.Now;
            
            _timePaused = TimeSpan.Zero;
        }

        public static void StartTimer()
        {
            _trackTime = true;
            _gameStartTime = DateTime.Now;
            _timeTracker = GameManager.instance.StartCoroutine(TrackTime(_gameStartTime));
        }
        
        public static void PauseTimer()
        {
            _pauseStartTime = DateTime.Now;
            StopTimer();
        }

        public static void ResumeTimer()
        {
            _pauseEndTime = DateTime.Now;
            _timePaused = _pauseEndTime - _pauseStartTime;
            
            _trackTime = true;
            _gameStartTime += _timePaused;
            _timeTracker = GameManager.instance.StartCoroutine(TrackTime(_gameStartTime));
        }
        
    
        public static void StopTimer()
        {
            _trackTime = false;
            OnTimerStop();
        }
    
        private static IEnumerator TrackTime(DateTime startingTime)
        {
            while (_trackTime)
            {
                if (_trackTime == false)
                {
                    OnTimerStop();
                    yield return null;
                }
                UpdateTimeDisplay(_timeText, startingTime);
                yield return new WaitForEndOfFrame();
            }
        }

        private static void OnTimerStop()
        {
            if (_timeTracker != null)
            {
                GameManager.instance.StopCoroutine(_timeTracker);
            }
        }

        private static void UpdateTimeDisplay(TMP_Text display, DateTime startingTime)
        {
            display.text = TimeDisplayPrefix + TrackTimeFrom(startingTime).ToString(@"m\:ss\.ff");
        }
    
        private static TimeSpan TrackTimeFrom(DateTime originalTime)
        {
            return(DateTime.Now - originalTime);
        }
    }
}
