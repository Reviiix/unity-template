using Abstract;
using MostlyPureFunctions;
using Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using UserInterface.ConditionalMenus;
using UserInterface.InGameMenus;
using UserInterface.MainMenus;
using UserInterface.MainMenus.StageSelection;
using UserInterface.PopUpMenus;

namespace UserInterface
{
    public class UserInterfaceManager : PrivateSingleton<UserInterfaceManager>
    {
        public static MonoBehaviour CoRoutineHandler => Instance;
        public static RawImage TransitionalFadeImage { get; private set; } //This image is attached the user interface game object and is used to achieve a fade effect by modifying the alpha value.
        [SerializeField] private Canvas background;
        [SerializeField] private Button returnToMainMenuButton;
        [Header("Main Menus")]
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private SettingsMenu settingsMenu;
        [SerializeField] private StageSelectionMenu stageSelectionMenu;
        [SerializeField] private StatisticsMenu statisticsMenu;
        [SerializeField] private StoreMenu storeMenu;
        [Header("In Game Menus")]
        [SerializeField] private InGameUserInterface inGameUserInterface;
        [SerializeField] private PauseUserMenu pauseUserMenu;
        [SerializeField] private GameOverMenu gameOverMenu;
        [Header("Pop Up Menus")]
        public ConfirmationPopUp confirmationScreen;
        public static ConfirmationPopUp ConfirmationScreen => Instance.confirmationScreen;
        private const string FirstOpenPopUpGuid = "Assets/Prefabs/UserInterface/ConditionalMenus/FirstOpenPopUp.prefab";
        private static readonly AssetReference FirstOpenPopUp = new AssetReference(FirstOpenPopUpGuid);
        private const string BirthdayPopUpGuid = "Assets/Prefabs/UserInterface/ConditionalMenus/BirthdayPopUp.prefab";
        private static readonly AssetReference BirthdayPopUp = new AssetReference(BirthdayPopUpGuid);
        private const string DailyLogInPopUpGuid = "Assets/Prefabs/UserInterface/ConditionalMenus/DailyLogInPopUp.prefab";
        private static readonly AssetReference DailyLogInPopUp = new AssetReference(DailyLogInPopUpGuid);
        private const string AnniversaryPopUpPopUpGuid = "Assets/Prefabs/UserInterface/ConditionalMenus/AnniversaryPopUp.prefab";
        private static readonly AssetReference AnniversaryPopUp = new AssetReference(AnniversaryPopUpPopUpGuid);

        public void Initialise()
        {
            ResolveDependencies();
            AssignButtonEvents();
            InitialiseMenus();
            StartUpSequence();
        }
        
        private void ResolveDependencies()
        {
            TransitionalFadeImage = GetComponent<RawImage>();
        }
        
        private void AssignButtonEvents()
        {
            returnToMainMenuButton.onClick.AddListener(()=>EnableMainMenu());
        }

        private void InitialiseMenus()
        {
            //Main Menus
            mainMenu.Initialise(()=>EnableStageSelectionMenu(), ()=>EnableStatisticsMenu(), ()=>EnableSettingsMenu(), ()=>EnableStoreMenu());
            settingsMenu.Initialise();
            statisticsMenu.Initialise();
            storeMenu.Initialise();
            stageSelectionMenu.Initialise();
            //In game UI
            inGameUserInterface.Initialise();
            gameOverMenu.Initialise();
            pauseUserMenu.Initialise();
        }
        
        private void StartUpSequence()
        {
            StartCoroutine(Wait.WaitForAnyAsynchronousInitialisationToComplete(() =>
            {
                EnableMainMenu();

                if (PlayerEngagementManager.TimesGameHasBeenOpened == 1)
                {
                    ShowFirstOpenPopUp();
                    return;
                }

                if (HolidayManager.IsAnniversary) ShowAnniversaryPopUp();

                if (HolidayManager.IsUserBirthday) ShowBirthdayPopUp();

                if (!PlayerEngagementManager.IsRepeatOpenToday) ShowDailyLogInPopUp();
                
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
            gameOverMenu.display.enabled = state;
            pauseUserMenu.display.enabled = state;
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

        private static void ShowBirthdayPopUp()
        {
            OpenAddressableMenu<BirthdayPopUp>(BirthdayPopUp);
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

        #region In Game Menus
        private void EnablePauseMenu(bool state = true)
        {
            pauseUserMenu.Enable(state);
        }
    
        public static void EnableGameOverMenu(bool state = true)
        {
            var instance = Instance;
            instance.gameOverMenu.Enable(state);
            EnableHeadsUpDisplay(false);
        }
        
        public static void EnableHeadsUpDisplay(bool state = true)
        {
            Instance.inGameUserInterface.Enable(state);
        }
        #endregion In Game Menus
    }
}