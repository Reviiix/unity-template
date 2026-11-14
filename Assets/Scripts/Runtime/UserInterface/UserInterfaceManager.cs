using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Engagement;
using UnityTemplate.UserInterface.ConditionalMenus;
using UnityTemplate.UserInterface.MainMenus;
using UnityTemplate.UserInterface.MainMenus.StageSelection;
using UnityTemplate.UserInterface.PopUpMenus;

namespace UnityTemplate.UserInterface
{
    /// <summary>Owns every menu and popup: switches menus, queues the start-up popups, runs the stage fade and handles the Android back button.</summary>
    [RequireComponent(typeof(RawImage))]
    public class UserInterfaceManager : MonoBehaviour
    {
        [SerializeField] private Canvas background;
        [SerializeField] private Button returnToMainMenuButton;

        [Header("Menus")]
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private StageSelectionMenu stageSelectionMenu;
        [SerializeField] private StatisticsMenu statisticsMenu;
        [SerializeField] private SettingsMenu settingsMenu;
        [SerializeField] private StoreMenu storeMenu;

        [Header("Pop Up Menus")]
        [SerializeField] private ConfirmationPopUp confirmationScreen;
        [SerializeField] private AssetReferenceGameObject firstOpenPopUp;
        [SerializeField] private AssetReferenceGameObject dailyLogInPopUp;
        [SerializeField] private AssetReferenceGameObject anniversaryPopUp;

        private readonly List<PopUpWindow> openPopUps = new();
        private MenuBase[] allMenus;
        private RawImage transitionalFadeImage;

        public ConfirmationPopUp ConfirmationScreen => confirmationScreen;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) HandleBackButton();
        }

        /// <summary>Wires buttons and lets each menu resolve its services; called once by GameInitialiser.</summary>
        public void Initialise()
        {
            transitionalFadeImage = GetComponent<RawImage>();
            transitionalFadeImage.enabled = false;
            allMenus = new MenuBase[] { mainMenu, stageSelectionMenu, statisticsMenu, settingsMenu, storeMenu };
            returnToMainMenuButton.onClick.AddListener(() => ShowMenu(mainMenu));
            mainMenu.Initialise(() => ShowMenu(stageSelectionMenu), () => ShowMenu(statisticsMenu), () => ShowMenu(settingsMenu), () => ShowMenu(storeMenu));
            stageSelectionMenu.Initialise();
            statisticsMenu.Initialise();
            settingsMenu.Initialise(this);
            storeMenu.Initialise();
        }

        /// <summary>Opens the main menu, then the first-open popup, or the anniversary and daily bonus popups one after another.</summary>
        public async Awaitable ShowStartUpPopUpsAsync(bool dailyBonusEnabled)
        {
            ShowMenu(mainMenu);

            var engagement = ServiceLocator.Get<EngagementService>();
            var today = System.DateTime.Now;
            if (engagement.IsFirstOpen)
            {
                await OpenAddressablePopUpAsync(firstOpenPopUp);
                return;
            }
            if (engagement.IsAnniversaryAvailable(today)) await OpenAddressablePopUpAsync(anniversaryPopUp);
            if (dailyBonusEnabled && engagement.IsDailyBonusAvailable(today)) await OpenAddressablePopUpAsync(dailyLogInPopUp);
        }

        /// <summary>Opens a popup that already lives in the scene and waits until it has closed.</summary>
        public async Awaitable OpenPopUpAsync(PopUpWindow popUp)
        {
            var closed = new AwaitableCompletionSource();
            void OnClosed(PopUpWindow _) => closed.TrySetResult();
            popUp.Closed += OnClosed;
            openPopUps.Add(popUp);
            popUp.Open();
            await closed.Awaitable;
            popUp.Closed -= OnClosed;
            openPopUps.Remove(popUp);
        }

        /// <summary>Destroys a popup instance created from an Addressable prefab and releases its asset.</summary>
        public void ReleasePopUp(GameObject popUpInstance)
        {
            if (!Addressables.ReleaseInstance(popUpInstance)) Destroy(popUpInstance);
        }

        /// <summary>Fades the screen to black (alpha 1) or back to clear (alpha 0) over the given time.</summary>
        public async Awaitable FadeScreenAsync(float alpha, float seconds)
        {
            var finished = new AwaitableCompletionSource();
            transitionalFadeImage.enabled = true;
            DOTween.To(() => transitionalFadeImage.color.a, SetFadeAlpha, alpha, seconds)
                .SetEase(Ease.Linear)
                .SetLink(gameObject)
                .OnKill(() => finished.TrySetResult());
            await finished.Awaitable;
            if (alpha <= 0) transitionalFadeImage.enabled = false;
        }

        private void SetFadeAlpha(float alpha)
        {
            var colour = transitionalFadeImage.color;
            colour.a = alpha;
            transitionalFadeImage.color = colour;
        }

        /// <summary>Hides the menu background and every menu so a stage scene can take over the screen.</summary>
        public void HideMenusForStage()
        {
            background.enabled = false;
            HideAllMenus();
        }

        private async Awaitable OpenAddressablePopUpAsync(AssetReferenceGameObject popUpReference)
        {
            if (!popUpReference.RuntimeKeyIsValid()) return;

            var instance = await Addressables.InstantiateAsync(popUpReference).Task;
            if (instance == null) return;

            await OpenPopUpAsync(instance.GetComponent<PopUpWindow>());
        }

        private void ShowMenu(MenuBase menu)
        {
            HideAllMenus();
            returnToMainMenuButton.interactable = menu != mainMenu;
            menu.Show();
        }

        private void HideAllMenus()
        {
            foreach (var menu in allMenus) menu.Hide();
        }

        private void HandleBackButton()
        {
            if (openPopUps.Count > 0)
            {
                openPopUps[^1].PressBack();
                return;
            }
            if (!mainMenu.IsVisible && background.enabled) ShowMenu(mainMenu);
        }
    }
}
