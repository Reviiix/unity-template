using System.Collections;
using System.Collections.Generic;
using Abstract;
using Abstract.Interfaces;
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Statistics.Experience
{
    /// <summary>
    /// This class displays the amount of experience based on the ExperienceManager class
    /// </summary>
    public class ExperienceDisplay : MonoBehaviour, IHandleSimultaneousAdditions
    {
        private bool animating;
        private Coroutine gainExperienceCoroutine;
        private static long _amountOfExperienceForNextLevelCache;
        private int levelIDCache;
        private static readonly Queue<KeyValuePair<long, long>> AdditionQueue = new Queue<KeyValuePair<long, long>>(); //Experience additions that happened during this animation are queued up and applied after the animation has finished.
        [SerializeField] private Image levelBar;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text experienceText;
        
        private IEnumerator Start()
        {
            yield return ProjectManager.WaitForInitialisation;
            Initialise();
        }

        private void OnEnable()
        {
            ExperienceManager.OnExperienceChange += OnExperienceChange;
        }

        private void OnDisable()
        {
            ExperienceManager.OnExperienceChange -= OnExperienceChange;
        }

        private void Initialise()
        {
            LoadSavedData();
            SetInitialDisplayState();
        }

        private void LoadSavedData()
        {
            levelIDCache = ExperienceManager.CurrentLevelID;
            _amountOfExperienceForNextLevelCache = ExperienceManager.GetAllExperienceToLevelUp(levelIDCache);
        }

        private void SetInitialDisplayState()
        {
            var totalExperience = ExperienceManager.TotalExperience;
            levelText.text = (levelIDCache + 1).ToString();
            levelBar.fillAmount = ReturnExperienceAsBarValue(totalExperience, levelIDCache);
            experienceText.text = totalExperience + "/" + _amountOfExperienceForNextLevelCache;
        }

        private void OnExperienceChange(long oldValue, long newValue)
        {
            if (animating)
            {
                AdditionQueue.Enqueue(new KeyValuePair<long, long>(oldValue, newValue));
                return;
            }
            gainExperienceCoroutine = StartCoroutine(GainExperienceAnimation(oldValue, newValue));
        }

        private IEnumerator GainExperienceAnimation(long oldValue, long newValue)
        {
            const float levelBarIncrementAmount = 0.01f;
            var newBarValue = ReturnExperienceAsBarValue(newValue, levelIDCache);
            var barValueDifference = newBarValue - levelBar.fillAmount;
            var amountOfWhileLoops = Mathf.CeilToInt(barValueDifference / levelBarIncrementAmount);
            var incrementAmount = Mathf.CeilToInt((float)(newValue - oldValue) / amountOfWhileLoops);
            var experienceAmount = (int)oldValue;
            animating = true;

            while (levelBar.fillAmount < newBarValue)
            {
                levelBar.fillAmount += levelBarIncrementAmount;
                experienceText.text = experienceAmount + "/" + _amountOfExperienceForNextLevelCache;
                experienceAmount += incrementAmount;

                if (levelBar.fillAmount >= 1) OnVisualLevelUp(experienceAmount, newValue);

                yield return null;
            }
            experienceText.text = newValue + "/" + _amountOfExperienceForNextLevelCache;
            levelBar.fillAmount = newBarValue;
            animating = false;
            
            StartCoroutine(HandleDelayedAdditions());
        }

        private void OnVisualLevelUp(long currentValue, long newValue)
        {
            if (gainExperienceCoroutine != null) StopCoroutine(gainExperienceCoroutine);
            levelIDCache++;
            _amountOfExperienceForNextLevelCache = ExperienceManager.GetAllExperienceToLevelUp(levelIDCache);
            levelText.text = (levelIDCache+1).ToString();
            levelBar.fillAmount = 0;
            RestartExperienceGainProcess(currentValue, newValue);
        }

        private void RestartExperienceGainProcess(long currentValue, long newValue)
        {
            gainExperienceCoroutine = StartCoroutine(GainExperienceAnimation(currentValue, newValue));
        }

        public IEnumerator HandleDelayedAdditions()
        {
            yield return new WaitUntil(() => !animating);
            
            if (AdditionQueue.Count == 0) yield break;

            var dequeue = AdditionQueue.Dequeue();
            RestartExperienceGainProcess(dequeue.Key, dequeue.Value);
        }

        private static float ReturnExperienceAsBarValue(long totalExperience, int levelID)
        {
            var previousLevelID = levelID - 1;
            var previousExperienceToLevelUp = previousLevelID >= 0 ? ExperienceManager.GetAllExperienceToLevelUp(previousLevelID) : 0;
            
            if (previousLevelID < 0) previousExperienceToLevelUp = 0;
            
            return (totalExperience - previousExperienceToLevelUp) * (100f / ExperienceManager.ReturnExperienceForLevelID(levelID)) / 100f;
        }
    }
}
