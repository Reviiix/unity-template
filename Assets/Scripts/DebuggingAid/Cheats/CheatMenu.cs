using System;
using Achievements;
using Credits;
using Player;
using Statistics;
using Statistics.Experience;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebuggingAid.Cheats
{
    [RequireComponent(typeof(Canvas))]
    public class CheatMenu : MonoBehaviour
    {
        #if UNITY_EDITOR || DEBUG_BUILD
        private static Canvas _cheatMenu;
        [SerializeField] private Button showCheatMenuButton;
        [Header("Experience Cheats")]
        [SerializeField] private Button addExperienceButton;
        [SerializeField] private TMP_InputField experienceInputField;
        [Header("Score Cheats")]
        [SerializeField] private Button addScoreButton;
        [SerializeField] private TMP_InputField scoreInputField;
        [Header("Credit Cheats")]
        [SerializeField] private Button addCreditsButton;
        [SerializeField] private TMP_InputField creditInputField;
        [Header("Cash Cheats")]
        [SerializeField] private Button addCashButton;
        [SerializeField] private TMP_Dropdown cashDropDown;
        [Header("Reset")]
        [SerializeField] private Button resetPlayerInformation;

        private void Awake()
        {
            Validate();
            ResolveDependencies();
            AssignButtonEvents();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space)) AchievementManager.UnlockAchievement(AchievementManager.Achievement.NumeroUno);
        }

        private void ResolveDependencies()
        {
            _cheatMenu = GetComponent<Canvas>();
        }

        private void AssignButtonEvents()
        {
            showCheatMenuButton.onClick.AddListener(OpenCheatMenuButtonPressed);
            
            addExperienceButton.onClick.AddListener(AddExperience);
            addScoreButton.onClick.AddListener(AddScore);
            resetPlayerInformation.onClick.AddListener(DeleteSaveData);
            addCreditsButton.onClick.AddListener(AddCredits);
            addCashButton.onClick.AddListener(AddCash);
        }
        
        private void OpenCheatMenuButtonPressed()
        {
            EnableCheatMenu(!GetComponent<Canvas>().isActiveAndEnabled);
        }
        
        private static void EnableCheatMenu(bool state = true)
        {
            _cheatMenu.enabled = state;
        }

        private void AddExperience()
        {
            var experienceAsString = experienceInputField.text;
            if (string.IsNullOrEmpty(experienceAsString)) return;
            var experience = int.Parse(experienceAsString);
            Debug.LogWarning("CHEATING! Adding: " + experience + " to experience.");
            ExperienceManager.AddExperience(experience);
        }
        
        private void AddScore()
        {
            var scoreAsString = scoreInputField.text;
            if (string.IsNullOrEmpty(scoreAsString)) return;
            var score = int.Parse(scoreAsString);
            Debug.LogWarning("CHEATING! Adding: " + score + " to score.");
            GameManager.Instance.IncrementScore(score);
        }
        
        private void AddCash()
        {
            var dropDownChoice = cashDropDown.value;
            throw new NotImplementedException();
        }
        
        private void AddCredits()
        {
            var creditsAsString = creditInputField.text;
            if (string.IsNullOrEmpty(creditsAsString)) return;
            var credits = int.Parse(creditsAsString);
            Debug.LogWarning("CHEATING! Adding: " + credits + " to credits.");
            CreditsManager.ChangeCredits(CreditsManager.Currency.PremiumCredits, credits);
        }

        private void Validate()
        {
            #if !UNITY_EDITOR && !DEBUG_BUILD 
            Destroy(gameObject);
            #endif
            Debugging.DisplayDebugMessage("CHEATS ENABLED! \n Culprit: " + name);
        }
        
        [ContextMenu("Delete Save Data")]
        public void DeleteSaveData()
        {
            SaveSystem.Delete();
            SaveSystem.Save();
        }
        
        #else
        private void OnEnable()
        {
            Destroy(gameObject);
        }
        #endif
    }
}
