using System;
using System.Collections;
using PureFunctions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Experience
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] private Image levelBar;
        [SerializeField] private TMP_Text levelText;
        [SerializeField] private TMP_Text experienceText;
        private static bool _animating;
        private static readonly WaitUntil WaitForAnimatingToFinish = new WaitUntil(() => !_animating);
        private static float ExperienceRepresentedAsBarValue => ExperienceManager.CurrentLevelExperience * (100f / ExperienceManager.ReturnExperienceForCurrentLevel()) / 100f;
        private static long _experienceCache;

        private void Start()
        {
            Initialise();
        }

        private void Initialise()
        {
            levelBar.fillAmount = ExperienceRepresentedAsBarValue;
            levelText.text = PlayerInformation.Level.ToString();
            experienceText.text = PlayerInformation.TotalExperience + "/" + ExperienceManager.ReturnAllPreviousExperienceForCurrentLevel();
            _experienceCache = PlayerInformation.TotalExperience;
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space)) ExperienceManager.AddExperience(150);
        }

        private void OnEnable()
        {
            ExperienceManager.OnExperienceChange += OnExperienceChange;
            ExperienceManager.OnLevelChange += OnLevelChange;
        }

        private void OnDisable()
        {
            ExperienceManager.OnExperienceChange -= OnExperienceChange;
            ExperienceManager.OnLevelChange -= OnLevelChange;
        }
        
        private void OnExperienceChange(long experienceGained)
        {
            StartCoroutine(Wait.WaitThenCallBack(0.1f, () =>
            {
                UpdateExperienceBar();
                UpdateExperienceTextWrapper();
            }));
        }

        private void OnLevelChange(int levelID)
        {
            WaitForAnimatingToStopWrapper(() =>
            {
                levelBar.fillAmount = 0;
                levelText.text = (levelID + 1).ToString();
            });
        }

        public static void WaitForAnimatingToStopWrapper(Action callBack)
        {
            ProjectManager.Instance.StartCoroutine(WaitForAnimatingToStop(callBack));
        }

        private static IEnumerator WaitForAnimatingToStop(Action callBack)
        {
            yield return WaitForAnimatingToFinish;
            callBack();
        }

        private void UpdateExperienceTextWrapper()
        {
            StartCoroutine(UpdateExperienceText());
        }

        private IEnumerator UpdateExperienceText()
        {
            while (_experienceCache <= PlayerInformation.TotalExperience)
            {
                //if (!_animating) _experienceCache = PlayerInformation.TotalExperience;
                experienceText.text = _experienceCache + "/" + ExperienceManager.ReturnAllPreviousExperienceForCurrentLevel(); //Do not cache in case the amount changes mid roll
                _experienceCache++;
                yield return null;
            }
            _experienceCache = PlayerInformation.TotalExperience;
            experienceText.text = _experienceCache + "/" + ExperienceManager.ReturnAllPreviousExperienceForCurrentLevel();
        }

        private float ReturnFillIncrementThatMatchesTextRollupSpeed()
        {
            return (ExperienceRepresentedAsBarValue - levelBar.fillAmount) / (PlayerInformation.TotalExperience - _experienceCache);
        }

        private void UpdateExperienceBar()
        {
            _animating = true;
            StartCoroutine(ChangeImageFillAmount.FillImage(levelBar, ExperienceRepresentedAsBarValue, ReturnFillIncrementThatMatchesTextRollupSpeed(), () =>
            {
                _animating = false;
            }));
        }
    }
}
