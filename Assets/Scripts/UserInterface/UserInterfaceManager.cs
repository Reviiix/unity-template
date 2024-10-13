using Player;
using pure_unity_methods;
using PureFunctions.UnitySpecific;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UserInterface.ConditionalMenus;
using UserInterface.MainMenus;
using UserInterface.MainMenus.StageSelection;
using UserInterface.PopUpMenus;

namespace UserInterface
{
    /// <summary>
    /// This class manages what menus will be open when.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class UserInterfaceManager : Singleton<UserInterfaceManager>
    {
        public RawImage transitionalFadeImage;
        [SerializeField] private Canvas background;
        [SerializeField] private Button returnToMainMenuButton;
        [Header("Menus")] //Menus
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private StageSelectionMenu stageSelectionMenu;
        [SerializeField] private StatisticsMenu statisticsMenu;
        [SerializeField] private SettingsMenu settingsMenu;
        [SerializeField] private StoreMenu storeMenu;
        [Header("Pop Up Menus")] //Pop Ups
        [SerializeField] private ConfirmationPopUp confirmationScreen;
        public static ConfirmationPopUp ConfirmationScreen => Instance.confirmationScreen;
        //Conditional Menus
        private static readonly AssetReference FirstOpenPopUp = new ("Prefabs/UserInterface/ConditionalMenus/FirstOpenPopUp.prefab");
        private static readonly AssetReference DailyLogInPopUp = new ("Prefabs/UserInterface/ConditionalMenus/DailyLogInPopUp.prefab");
        private static readonly AssetReference AnniversaryPopUp = new ("Prefabs/UserInterface/ConditionalMenus/AnniversaryPopUp.prefab");

        public void Initialise()
        {
            AssignButtonEvents();
            InitialiseMenus();
            StartUpSequence();
        }

        private void AssignButtonEvents()
        {
            returnToMainMenuButton.onClick.AddListener(()=>EnableMainMenu());
        }

        private void InitialiseMenus()
        {
            mainMenu.Initialise(()=>EnableStageSelectionMenu(), ()=>EnableStatisticsMenu(), ()=>EnableSettingsMenu(), ()=>EnableStoreMenu());
            settingsMenu.Initialise();
            statisticsMenu.Initialise();
            storeMenu.Initialise();
            stageSelectionMenu.Initialise();
        }
        
        private void StartUpSequence()
        {
            StartCoroutine(Wait.WaitForInitialisationToComplete(() =>
            {
                EnableMainMenu();

                if (PlayerEngagement.TimesGameHasBeenOpened == 1)
                {
                    ShowFirstOpenPopUp();
                    return;
                }

                if (HolidayManager.IsAnniversary)
                {
                    ShowAnniversaryPopUp();
                }
                if (!PlayerEngagement.IsRepeatOpenToday)
                {
                    ShowDailyLogInPopUp();
                }
            }));
        }

        #region Main Menus
        private void EnableMainMenu(bool state = true)
        {
            returnToMainMenuButton.interactable = !state;
            EnableAllNonPermanentCanvases(false);
            mainMenu.Enable(state);
        }
        
        private void EnableStageSelectionMenu(bool state = true)
        {
            EnableAllNonPermanentCanvases(false);
            returnToMainMenuButton.interactable = state;
            stageSelectionMenu.Enable(state);
        }
        
        private void EnableStoreMenu(bool state = true)
        {
            EnableAllNonPermanentCanvases(false);
            returnToMainMenuButton.interactable = state;
            storeMenu.Enable(state);
        }
        
        private void EnableStatisticsMenu(bool state = true)
        {
            EnableAllNonPermanentCanvases(false);
            returnToMainMenuButton.interactable = state;
            statisticsMenu.Enable(state);
        }
        
        private void EnableSettingsMenu(bool state = true)
        {
            EnableAllNonPermanentCanvases(false);
            returnToMainMenuButton.interactable = state;
            settingsMenu.Enable(state);
        }
        
        public static void EnableBackground(bool state = true)
        {
            Instance.background.enabled = state;
        }
        
        public static void EnableAllNonPermanentCanvasesStaticCall(bool state = true)
        {
            Instance.EnableAllNonPermanentCanvases(state);
        }

        private void EnableAllNonPermanentCanvases(bool state = true)
        {
            mainMenu.display.enabled = state;
            settingsMenu.display.enabled = state;
            storeMenu.display.enabled = state;
            statisticsMenu.display.enabled = state;
            stageSelectionMenu.display.enabled = state;
        }
        #endregion

        #region Conditional Menus
        private static void ShowFirstOpenPopUp()
        {
            OpenAddressableMenu<FirstOpenPopUpMenu>(FirstOpenPopUp);
        }
        private static void ShowDailyLogInPopUp()
        {
            OpenAddressableMenu<DailyLogInPopUp>(DailyLogInPopUp);
        }

        private static void ShowAnniversaryPopUp()
        {
            OpenAddressableMenu<AnniversaryPopUp>(AnniversaryPopUp);
        }

        private static void OpenAddressableMenu<T>(IKeyEvaluator menuReference) where T : IUserInterface
        {
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(menuReference, (returnVariable) =>
            {
                var menu = Instantiate(returnVariable).GetComponent<T>();
                AssetReferenceLoader.DestroyOrUnload(returnVariable);
                menu.Enable();
            });
        }
        #endregion Conditional Menus
    }
}