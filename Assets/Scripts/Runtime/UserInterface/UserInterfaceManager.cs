using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UnityTemplate.Core;
using UnityTemplate.Engagement;
using UnityTemplate.Progression;
using UnityTemplate.UserInterface.ConditionalMenus;
using UnityTemplate.UserInterface.MainMenus;
using UnityTemplate.UserInterface.MainMenus.StageSelection;
using UnityTemplate.UserInterface.PopUpMenus;
using UnityTemplate.Utilities;

namespace UnityTemplate.UserInterface
{
    /// <summary>Owns every menu and popup: switches menus, queues the start-up popups, runs the stage fade, hides and restores the menus around stages and handles BACK.</summary>
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
        [SerializeField] private NoticeToast noticeToast;
        [SerializeField] private AssetReferenceGameObject firstOpenPopUp;
        [SerializeField] private AssetReferenceGameObject dailyLogInPopUp;
        [SerializeField] private AssetReferenceGameObject anniversaryPopUp;

        private readonly List<PopUpWindow> openPopUps = new();
        private readonly MenuRenderThrottle renderThrottle = new();
        private MenuBase[] allMenus;
        private GameObject[] menuBackdrop;
        private StageLoader stageLoader;
        private RawImage transitionalFadeImage;

        public ConfirmationPopUp ConfirmationScreen => confirmationScreen;
        public bool IsShowingStartUpPopUps { get; private set; }
        public bool IsInStage { get; private set; }

        /// <summary>Polls the Android back button (the old Input Manager offers no event for it) and throttles rendering while the menus are still.</summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) HandleBackButton();
            renderThrottle.Tick(!IsInStage);
        }

        /// <summary>Returns rendering to every frame when the user interface goes away.</summary>
        private void OnDestroy()
        {
            renderThrottle.Stop();
        }

        /// <summary>Wires buttons and lets each menu resolve its services; called once by GameInitialiser.</summary>
        public void Initialise()
        {
            transitionalFadeImage = GetComponent<RawImage>();
            transitionalFadeImage.enabled = false;
            allMenus = new MenuBase[] { mainMenu, stageSelectionMenu, statisticsMenu, settingsMenu, storeMenu };
            menuBackdrop = MenuBackdrop();
            returnToMainMenuButton.onClick.AddListener(PressBack);
            mainMenu.Initialise(() => ShowMenu(stageSelectionMenu), () => ShowMenu(statisticsMenu), () => ShowMenu(settingsMenu), () => ShowMenu(storeMenu));
            stageSelectionMenu.Initialise();
            statisticsMenu.Initialise();
            settingsMenu.Initialise(this);
            storeMenu.Initialise();
        }

        /// <summary>Opens the main menu, then the first-open popup, or the anniversary and daily bonus popups one after another; stages cannot start until it finishes.</summary>
        public async Awaitable ShowStartUpPopUpsAsync(bool dailyBonusEnabled)
        {
            ShowMenu(mainMenu);
            IsShowingStartUpPopUps = true;
            try
            {
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
            finally
            {
                IsShowingStartUpPopUps = false;
            }
        }

        /// <summary>Shows a short message at the bottom of the screen for the given number of seconds; taps pass through it.</summary>
        public void ShowNotice(string text, float seconds)
        {
            noticeToast.Show(text, seconds);
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
            if (transitionalFadeImage != null && alpha <= 0) transitionalFadeImage.enabled = false;
        }

        /// <summary>Sets the fade image's alpha.</summary>
        private void SetFadeAlpha(float alpha)
        {
            var colour = transitionalFadeImage.color;
            colour.a = alpha;
            transitionalFadeImage.color = colour;
        }

        /// <summary>Hides every menu and the menu backdrop so a stage scene can take over the screen; the BACK button stays as the way out.</summary>
        public void HideMenusForStage()
        {
            IsInStage = true;
            SetMenuBackdropVisible(false);
            HideAllMenus();
            returnToMainMenuButton.interactable = true;
        }

        /// <summary>Shows the menu backdrop and the main menu again after a stage has been unloaded.</summary>
        public void ShowMenusAfterStage()
        {
            IsInStage = false;
            SetMenuBackdropVisible(true);
            ShowMenu(mainMenu);
        }

        /// <summary>Instantiates an Addressable popup under the user interface root and waits until it has closed.</summary>
        private async Awaitable OpenAddressablePopUpAsync(AssetReferenceGameObject popUpReference)
        {
            if (!popUpReference.RuntimeKeyIsValid()) return;

            var instance = await Addressables.InstantiateAsync(popUpReference).ToAwaitable();
            if (instance == null) return;

            await OpenPopUpAsync(instance.GetComponent<PopUpWindow>());
        }

        /// <summary>Shows one menu and hides the rest; BACK is only tappable away from the main menu.</summary>
        private void ShowMenu(MenuBase menu)
        {
            HideAllMenus();
            returnToMainMenuButton.interactable = menu != mainMenu;
            menu.Show();
        }

        /// <summary>Returns everything on the background canvas except the BACK button, which stays visible in stages.</summary>
        private GameObject[] MenuBackdrop()
        {
            var children = new List<GameObject>();
            foreach (Transform child in background.transform)
            {
                if (child != returnToMainMenuButton.transform) children.Add(child.gameObject);
            }
            return children.ToArray();
        }

        /// <summary>Shows or hides the menu backdrop (background, card and version text).</summary>
        private void SetMenuBackdropVisible(bool visible)
        {
            foreach (var child in menuBackdrop) child.SetActive(visible);
        }

        /// <summary>BACK leaves a stage for the main menu, or returns from a sub-menu to the main menu.</summary>
        private void PressBack()
        {
            if (IsInStage)
            {
                stageLoader ??= ServiceLocator.Get<StageLoader>();
                stageLoader.ReturnToMainMenu();
                return;
            }
            ShowMenu(mainMenu);
        }

        /// <summary>Hides every menu.</summary>
        private void HideAllMenus()
        {
            foreach (var menu in allMenus) menu.Hide();
        }

        /// <summary>Closes the top popup, leaves a stage, or returns from a sub-menu to the main menu.</summary>
        private void HandleBackButton()
        {
            if (openPopUps.Count > 0)
            {
                openPopUps[^1].PressBack();
                return;
            }
            if (IsInStage || !mainMenu.IsVisible) PressBack();
        }
    }
}
