using System;
using Credits;
using Statistics.Experience;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DebuggingAid.Cheats
{
    /// <summary>
    /// This class cheats features! Mainly for debugging purposes.
    /// This class will be removed by the CheatEnabler class (or itself) when project is built (unless the DEBUG_BUILD scripting define symbol is enabled).
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CheatManager : MonoBehaviour
    {
        #if UNITY_EDITOR || DEBUG_BUILD
        public const bool Enabled = true;
        private const string AddingByCheatingMessage = "CHEATING! Adding: ";
        private static Canvas _cheatMenu;
        [FormerlySerializedAs("showCheatMenuButton")] [SerializeField] private Button openCheatMenuButton;
        [Header("Experience")]
        [SerializeField] private Button addExperienceButton;
        [SerializeField] private TMP_InputField experienceInputField;
        [Header("Score")]
        [SerializeField] private Button addScoreButton;
        [SerializeField] private TMP_InputField scoreInputField;
        [Header("Credit")]
        [SerializeField] private Button addCreditsButton;
        [SerializeField] private TMP_InputField creditInputField;
        [Header("Premium Credit")]
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
            openCheatMenuButton.onClick.AddListener(OpenCheatMenuButtonPressed);
            
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
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IncrementScore(score);
            }
            else
            {
                const string errorMessage = "No game manager to increment score.";
                Debug.LogError(errorMessage);
            }
        }

        public  void AddCredits()
        {
            var creditsAsString = creditInputField.text;
            if (string.IsNullOrEmpty(creditsAsString)) return;
            var credits = int.Parse(creditsAsString);
            const string cheatedItemMessage = " to credits.";
            Debug.LogWarning(AddingByCheatingMessage + credits + cheatedItemMessage);
            CreditsManager.AddCredits(CreditsManager.Currency.Credits, credits);
        }
        
        private void AddPremiumCredits()
        {
            var creditsAsString = premiumCreditInputField.text;
            if (string.IsNullOrEmpty(creditsAsString)) return;
            var credits = int.Parse(creditsAsString);
            const string cheatedItemMessage = " to premium credits.";
            Debug.LogWarning(AddingByCheatingMessage + credits + cheatedItemMessage);
            CreditsManager.AddCredits(CreditsManager.Currency.PremiumCredits, credits);
        }

        private void Validate(Action callBack)
        {
            #if !UNITY_EDITOR && !DEBUG_BUILD 
            Addressables.ReleaseInstance(gameObject);
            return;
            #endif
            if (Enabled)
            {
                const string errorMessage = "CHEATS ENABLED! \n Culprit: ";
                Debug.LogWarning(errorMessage + name);
                callBack();
                return;
            }
            openCheatMenuButton.gameObject.SetActive(false);
            Addressables.ReleaseInstance(gameObject);
        }

        #else
        private void OnEnable()
        {
            Addressables.ReleaseInstance(gameObject);
        }
        #endif
    }
}
