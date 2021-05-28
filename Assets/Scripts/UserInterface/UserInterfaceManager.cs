using Abstract;
using PureFunctions;
using UnityEngine;
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
        [Header("Pop Up Menus")]
        [SerializeField] private FirstOpenPopUpMenu firstOpenPopUpMenu;
        [SerializeField] private BirthdayPopUp birthdayPopUpMenu;
        [SerializeField] private DailyLogInPopUp dailyLogInPopUpMenu;
        [Header("In Game Menus")]
        [SerializeField] private InGameUserInterface inGameUserInterface;
        [SerializeField] private PauseUserMenu pauseUserMenu;
        [SerializeField] private GameOverMenu gameOverMenu;

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
            //Main Menu Sub Pages
            mainMenu.Initialise(()=>EnableStageSelectionMenu(), ()=>EnableStatisticsMenu(), ()=>EnableSettingsMenu(), ()=>EnableStoreMenu());
            settingsMenu.Initialise();
            statisticsMenu.Initialise();
            storeMenu.Initialise();
            stageSelectionMenu.Initialise();
            //In game UI
            inGameUserInterface.Initialise();
            gameOverMenu.Initialise();
            pauseUserMenu.Initialise();
            //Pop ups
            firstOpenPopUpMenu.Initialise();
            birthdayPopUpMenu.Initialise();
            dailyLogInPopUpMenu.Initialise();
        }
        
        private void StartUpSequence()
        {
            StartCoroutine(Wait.WaitForAnyAsynchronousInitialisationToComplete(() =>
            {
                EnableMainMenu();

                if (PlayerEngagementManager.TimesGameHasBeenOpened == 1)
                {
                    ShowWelcomeMessage();
                    return;
                }

                if (HolidayManager.IsUserBirthday) ShowBirthdayMessage();

                if (!PlayerEngagementManager.IsRepeatOpenToday) ShowDailyBonus();
            }));
        }

        private void ShowWelcomeMessage(int delayInSeconds = 1)
        {
            StartCoroutine(Wait.WaitThenCallBack(delayInSeconds, ()=>firstOpenPopUpMenu.Enable()));
        }
        
        private void ShowDailyBonus(int delayInSeconds = 1)
        {
            StartCoroutine(Wait.WaitThenCallBack(delayInSeconds, ()=>dailyLogInPopUpMenu.Enable()));
        }

        private void ShowBirthdayMessage(int delayInSeconds = 1)
        {
            StartCoroutine(Wait.WaitThenCallBack(delayInSeconds, ()=>birthdayPopUpMenu.Enable()));
        }

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

        private void PauseButtonPressed()
        {
            EnablePauseMenu(PauseManager.IsPaused);
        }

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
    }
}