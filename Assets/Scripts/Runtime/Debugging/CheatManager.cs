using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Economy;
using UnityTemplate.Gameplay;
using UnityTemplate.Progression;

namespace UnityTemplate.Debugging
{
    /// <summary>Development-only cheat menu for adding experience, score and currency; release builds destroy it on load.</summary>
    [RequireComponent(typeof(Canvas))]
    public class CheatManager : MonoBehaviour
    {
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
        private Canvas cheatMenu;

        /// <summary>Removes the menu from release builds; otherwise wires each cheat button to its input field.</summary>
        private void Awake()
        {
            if (!Debug.isDebugBuild)
            {
                Destroy(gameObject);
                return;
            }

            cheatMenu = GetComponent<Canvas>();
            openCheatMenuButton.onClick.AddListener(() => cheatMenu.enabled = !cheatMenu.enabled);
            addExperienceButton.onClick.AddListener(() => Apply(experienceInputField, amount => ServiceLocator.Get<ExperienceService>().Add(amount)));
            addScoreButton.onClick.AddListener(() => Apply(scoreInputField, AddScore));
            addCreditsButton.onClick.AddListener(() => Apply(creditInputField, amount => ServiceLocator.Get<CreditsService>().Add(Currency.Credits, amount)));
            addPremiumCreditsButton.onClick.AddListener(() => Apply(premiumCreditInputField, amount => ServiceLocator.Get<CreditsService>().Add(Currency.PremiumCredits, amount)));
            Debug.LogWarning("CHEATS ENABLED on " + name);
        }

        /// <summary>Parses the field's number and runs the cheat with it, ignoring empty or invalid input.</summary>
        private static void Apply(TMP_InputField inputField, Action<int> cheat)
        {
            if (!int.TryParse(inputField.text, out var amount)) return;

            Debug.LogWarning($"CHEATING! Adding {amount} via {inputField.name}.");
            cheat(amount);
        }

        /// <summary>Adds score to the running stage, if a stage is loaded.</summary>
        private static void AddScore(int amount)
        {
            if (ServiceLocator.TryGet<GameManager>(out var gameManager)) gameManager.IncrementScore(amount);
            else Debug.LogWarning("No GameManager is loaded, so score cannot be added outside a stage.");
        }
    }
}
