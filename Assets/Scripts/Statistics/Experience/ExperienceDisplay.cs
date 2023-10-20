using System.Collections;
using System.Collections.Generic;
using Abstract.Interfaces;
using DG.Tweening;
using PureFunctions;
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
        private static readonly Queue<KeyValuePair<long, long>> AdditionQueue = new (); //Experience additions that happened during this animation are queued up and applied after the animation has finished.
        [SerializeField] private Image levelBar;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text experienceText;
        private const int AnimationDuration = 3; 
        
        private IEnumerator Start()
        {
            yield return Wait.WaitForInitialisation;
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
            GainExperienceAnimation(oldValue, newValue);
        }

        private void GainExperienceAnimation(long oldValue, long newValue, int seconds = AnimationDuration)
        {
            ChangeFillBarAmount(newValue, seconds);
            ChangeTextAmount(oldValue, newValue, seconds);
        }

        private void ChangeFillBarAmount(long newValue, int seconds)
        {
            one = DOTween.To(()=> levelBar.fillAmount, x=> levelBar.fillAmount = x, ReturnExperienceAsBarValue(newValue, levelIDCache), seconds).OnComplete(() =>
            {
                //levelBar.fillAmount = ReturnExperienceAsBarValue(newValue, levelIDCache);
            });
        }
        
        private Tween one;
        private Tween two;
        
        private void ChangeTextAmount(long oldValue, long newValue, int seconds)
        {
            var numberDisplay = oldValue;

            two = DOTween.To(()=> numberDisplay, x=> numberDisplay = x, newValue, seconds).OnUpdate(() =>
            {
                experienceText.text = numberDisplay + "/" + _amountOfExperienceForNextLevelCache;
                if (numberDisplay >= _amountOfExperienceForNextLevelCache)
                {
                    DOTween.Kill(one);
                    DOTween.Kill(two);
                    levelIDCache++;
                    _amountOfExperienceForNextLevelCache = ExperienceManager.GetAllExperienceToLevelUp(levelIDCache);
                    levelText.text = (levelIDCache+1).ToString();
                    levelBar.fillAmount = 0;
                    //RestartExperienceGainProcess(numberDisplay, newValue);

                }
            }).OnComplete(() =>
            {
                //experienceText.text = newValue + "/" + _amountOfExperienceForNextLevelCache;
            });
        }
        

        private void RestartExperienceGainProcess(long currentValue, long newValue)
        {
            GainExperienceAnimation(currentValue, newValue);;
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
