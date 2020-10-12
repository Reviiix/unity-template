using System;
using System.Collections;
using Audio;
using JetBrains.Annotations;
using PureFunctions.Movement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class UserInterfaceManager : MonoBehaviour
    {
        public static RawImage TransitionalFadeImage { get; private set; }
        private static readonly Color[] PlayButtonColorSwaps = {Color.white, Color.black};
        private const int DefaultMenuMovementSpeed = 7;
        private const float MenuMovementDistanceTolerance = 1f;
        public static int ScreenDistance => ProjectManager.ReturnScreenWidth() * 2;
        [SerializeField]
        private IntroductionMenu introductionMenu;
        [SerializeField]
        private InGameMenu inGameMenu;
        [SerializeField]
        private PauseMenu pauseMenu;
        [SerializeField]
        private GameOverMenu gameOverMenu;
        [SerializeField]
        private SettingsMenu settingsMenu;

        private void Start()
        {
            Initialise();
        
            EnableStartMenu();
        }

        private void Initialise()
        {
            ResolveDependencies();

            InitialiseMenus();
        }

        private void InitialiseMenus()
        {
            introductionMenu.Initialise(()=>EnableAllNonPermanentCanvases(false), PlayButtonColorSwaps);
            gameOverMenu.Initialise(PlayButtonColorSwaps);
            pauseMenu.Initialise(PauseButtonPressed);
            settingsMenu.Initialise(()=> EnablePauseButton(), () => EnablePauseButton(false),()=>EnablePauseMenu());
        }

        private void ResolveDependencies()
        {
            TransitionalFadeImage = GetComponent<RawImage>();
        }

        public void EnableStartMenu()
        {
            introductionMenu.Enable();
        }

        public void EnableInGameUserInterface(bool state = true)
        {
            inGameMenu.Enable(state);
        }

        private void PauseButtonPressed()
        {
            EnablePauseMenu(PauseManager.IsPaused);
        }

        private void EnablePauseMenu(bool state = true)
        {
            pauseMenu.Enable(state);
        }
    
        public void EnableGameOverMenu(bool state = true)
        {
            gameOverMenu.Enable(state);
        }
    
        public void EnablePauseButton(bool state = true)
        {
            pauseMenu.EnablePauseButtons(state);
        }
    
        private void EnableAllNonPermanentCanvases(bool state = true)
        {
            introductionMenu.display.enabled = state;
            gameOverMenu.display.enabled = state;
            pauseMenu.display.enabled = state;
            settingsMenu.display.enabled = state;
        }

        public TMP_Text ReturnScoreText()
        {
            return inGameMenu.scoreText;
        }
    
        public TMP_Text ReturnTimeText()
        {
            return inGameMenu.timeText;
        }

        private static void TransformEnterToScreenCentre(Transform transformToMove, Action callBack = null, int speed = DefaultMenuMovementSpeed)
        {
            ProjectManager.Instance.StartCoroutine(MoveTransformToCentre(transformToMove, speed, ()=>
            {
                callBack?.Invoke();
            }));
        }
        
        private static void TransformExitLeft(Transform transformToMove, Action callBack = null, int speed = DefaultMenuMovementSpeed)
        {
            ProjectManager.Instance.StartCoroutine(MoveTransformToLeft(transformToMove, speed, ()=>
            {
                callBack?.Invoke();
            }));
        }
        
        public static void AppearAnimation(Transform popUpMenu, Action callBack = null)
        {
            if (popUpMenu == null)
            {
                Debugging.DisplayDebugMessage("There is no pop up menu assigned for this menu but you are still trying to animate it.");
                callBack?.Invoke();
                return;
            }

            popUpMenu.localPosition = new Vector3(-UserInterfaceManager.ScreenDistance,0,0);
            
            TransformEnterToScreenCentre(popUpMenu, ()=>
            {
                callBack?.Invoke();
            });
        }
        
        public static void DisappearAnimation(Transform popUpMenu, Action callBack = null)
        {
            if (popUpMenu == null)
            {
                Debugging.DisplayDebugMessage("There is no pop up menu assigned for this menu but you are still trying to animate it.");
                callBack?.Invoke();
                return;
            }
            TransformExitLeft(popUpMenu, ()=>
            {
                callBack?.Invoke();
            });
        }

        private static IEnumerator MoveTransformToCentre(Transform transformToMove, int speed = DefaultMenuMovementSpeed, Action callBack = null)
        {
            BaseAudioManager.PlayMenuMovementSound();
            while (Vector2.Distance(transformToMove.localPosition, Vector2.zero) >= MenuMovementDistanceTolerance)
            {
                MoveInAndOutOfView.MoveToCentre(transformToMove, speed);
                yield return null;
            }
            callBack?.Invoke();
        }
        
        private static IEnumerator MoveTransformToLeft(Transform transformToMove, int speed = DefaultMenuMovementSpeed, Action callBack = null)
        {
            var leftPosition = new Vector3(-ScreenDistance, 0 , 0);
            
            BaseAudioManager.PlayMenuMovementSound();
            while (Vector2.Distance(transformToMove.localPosition, leftPosition) >= MenuMovementDistanceTolerance)
            {
                MoveInAndOutOfView.MoveToLeft(transformToMove, speed, ScreenDistance);
                yield return null;
            }
            callBack?.Invoke();
        }
    }

    [Serializable]
    public abstract class Menu
    {
        public Canvas display;
        protected static Action DisableAllCanvases;
        [CanBeNull] [SerializeField]
        protected Transform popUpMenu;
    }

    public interface IMenu
    { 
        void Enable(bool state = true);
    }
}