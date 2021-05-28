using System;
using PureFunctions.Movement;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    [Serializable]
    public abstract class UserInterface
    {
        public Canvas display;
        protected static MonoBehaviour CoRoutineHandler => UserInterfaceManager.CoRoutineHandler;
    }
    
    public interface IUserInterface
    { 
        void Enable(bool state = true);
    }
    
    [Serializable]
    public abstract class PopUpInterface : UserInterface
    {
        [Header("Pop Up")]
        [SerializeField] protected Transform popUpMenu;
        [SerializeField] protected Button closeMenu;
        private const int DefaultMenuMovementSpeed = 7;
        private const int ScreenDistanceBuffer = 500; //ScreenDistanceBuffer is intended to get the remaining bit of the object off screen if its centre is placed at the screen edge.
        private static int ScreenDistance => (CameraManager.ReturnScreenWidth * 2) + ScreenDistanceBuffer;

        public virtual void Initialise()
        {
            SetCloseButtonEvent();
        }

        private void SetCloseButtonEvent()
        {
            closeMenu.onClick.AddListener(CloseButtonPressed);
        }

        protected virtual void CloseButtonPressed()
        {
            DisappearAnimation(popUpMenu, () =>
            {
                display.enabled = false;
            });
        }
        
        private static void TransformEnterToScreenCentre(Transform transformToMove, Action callBack = null, int speed = DefaultMenuMovementSpeed)
        {
            CoRoutineHandler.StartCoroutine(MoveInAndOutOfView.MoveTransformToCentre(transformToMove, speed, 1,()=>
            {
                callBack?.Invoke();
            }));
        }
        
        private static void TransformExitLeft(Transform transformToMove, Action callBack = null, int speed = DefaultMenuMovementSpeed)
        {
            CoRoutineHandler.StartCoroutine(MoveInAndOutOfView.MoveTransformToLeft(transformToMove, speed, new Vector3(ScreenDistance,0), 1,()=>
            {
                callBack?.Invoke();
            }));
        }
        
        protected static void AppearAnimation(Transform popUpMenu, Action callBack = null)
        {
            popUpMenu.localPosition = new Vector3(-ScreenDistance,0,0);
            
            TransformEnterToScreenCentre(popUpMenu, ()=>
            {
                callBack?.Invoke();
            });
        }
        
        protected static void DisappearAnimation(Transform popUpMenu, Action callBack = null)
        {
            TransformExitLeft(popUpMenu, ()=>
            {
                callBack?.Invoke();
            });
        }
    }
}