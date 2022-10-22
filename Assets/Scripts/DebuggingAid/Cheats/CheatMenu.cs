using System;
using Credits;
using Statistics.Experience;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebuggingAid.Cheats
{
    /// <summary>
    /// This class cheats features! Mainly for debugging purposes
    /// This class will be removed when project is built by the CheatEnabler class (unless the DEBUG_BUILD scripting define symbol is enabled).
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CheatMenu : MonoBehaviour
    {
        #if UNITY_EDITOR || DEBUG_BUILD
        private const string AddingByCheatingMessage = "CHEATING! Adding: ";
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
        [Header("Premium Credit Cheats")]
        [SerializeField] private Button addPremiumCreditsButton;
        [SerializeField] private TMP_InputField premiumCreditInputField;
        
        private void Awake()
        {
            Validate(() =>
            {
                ResolveDependencies();
                AssignButtonEvents();
            });
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
            addCreditsButton.onClick.AddListener(AddCredits);
            addPremiumCreditsButton.onClick.AddListener(AddPremiumCredits);
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
            const string cheatedItemMessage = " to experience.";
            Debug.LogWarning(AddingByCheatingMessage + experience + cheatedItemMessage);
            ExperienceManager.AddExperience(experience);
        }
        
        private void AddScore()
        {
            var scoreAsString = scoreInputField.text;
            if (string.IsNullOrEmpty(scoreAsString)) return;
            var score = int.Parse(scoreAsString);
            const string cheatedItemMessage = " to score.";
            Debug.LogWarning(AddingByCheatingMessage + score + cheatedItemMessage);
            GameManager.Instance.IncrementScore(score);
        }

        private void AddCredits()
        {
            var creditsAsString = creditInputField.text;
            if (string.IsNullOrEmpty(creditsAsString)) return;
            var credits = int.Parse(creditsAsString);
            const string cheatedItemMessage = " to credits.";
            Debug.LogWarning(AddingByCheatingMessage + credits + cheatedItemMessage);
            CreditsManager.IncrementCredits(CreditsManager.Currency.Credits, credits);
        }
        
        private void AddPremiumCredits()
        {
            var creditsAsString = premiumCreditInputField.text;
            if (string.IsNullOrEmpty(creditsAsString)) return;
            var credits = int.Parse(creditsAsString);
            const string cheatedItemMessage = " to premium credits.";
            Debug.LogWarning(AddingByCheatingMessage + credits + cheatedItemMessage);
            CreditsManager.IncrementCredits(CreditsManager.Currency.PremiumCredits, credits);
        }

        private void Validate(Action callBack)
        {
            #if !UNITY_EDITOR && !DEBUG_BUILD 
            Addressables.ReleaseInstance(gameObject);
            return;
            #endif
            const string errorMessage = "CHEATS ENABLED! \n Culprit: ";
            Debug.LogWarning(errorMessage + name);
            callBack();
        }

        #else
        private void OnEnable()
        {
            Addressables.ReleaseInstance(gameObject);
        }
        #endif
    }
}
