using System;
using Audio;
using PureFunctions.Movement;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    /// <summary>
    /// This is the base class for menus
    /// </summary>
    [Serializable]
    public abstract class UserInterface
    {
        public Canvas display;
        protected static MonoBehaviour CoRoutineHandler => UserInterfaceManager.CoRoutineHandler;
    }
    
    /// <summary>
    /// This is the interface for menus
    /// </summary>
    public interface IUserInterface
    { 
        void Enable(bool state = true);
    }
    
    /// <summary>
    /// This is the base class for pop up menus
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public abstract class PopUpInterface : MonoBehaviour
    {
        protected Canvas Display;
        [SerializeField] protected Transform popUpMenu;
        [SerializeField] protected Button closeButton;
        private const int DefaultMenuMovementSpeed = 7;
        private const int ScreenDistanceBuffer = 500; //ScreenDistanceBuffer is intended to get the remaining bit of the object off screen if its centre is placed at the screen edge.
        private static int ScreenDistance => (CameraManager.ReturnScreenWidth * 2) + ScreenDistanceBuffer;

        protected virtual void Awake()
        {
            Display = GetComponent<Canvas>();
            if (closeButton != null) closeButton.onClick.AddListener(CloseButtonPressed);
            Display.enabled = false;
        }
        
        public virtual void Enable(bool state = true)
        {
            switch (state)
            {
                case true:
                    Display.enabled = true;
                    AppearAnimation(popUpMenu);
                    break;
                case false:
                    DisappearAnimation(popUpMenu, UnloadSelf);
                    Display.enabled = false;
                    break;
            }
            BaseAudioManager.PlayMenuMovementSound();
        }

        private void CloseButtonPressed()
        {
            DisappearAnimation(popUpMenu, UnloadSelf);
        }
        
        private void TransformEnterToScreenCentre(Transform transformToMove, Action callBack = null, int speed = DefaultMenuMovementSpeed)
        {
            StartCoroutine(MoveInAndOutOfView.MoveTransformToCentre(transformToMove, speed, 1,()=>
            {
                callBack?.Invoke();
            }));
        }
        
        private void TransformExitLeft(Transform transformToMove, Action callBack = null, int speed = DefaultMenuMovementSpeed)
        {
            StartCoroutine(MoveInAndOutOfView.MoveTransformToLeft(transformToMove, speed, new Vector3(ScreenDistance,0), 1,()=>
            {
                callBack?.Invoke();
            }));
        }
        
        protected void AppearAnimation(Transform popUpTransform, Action callBack = null)
        {
            popUpTransform.localPosition = new Vector3(-ScreenDistance,0,0);
            
            TransformEnterToScreenCentre(popUpTransform, ()=>
            {
                callBack?.Invoke();
            });
        }
        
        protected void DisappearAnimation(Transform popUpTransform, Action callBack = null)
        {
            TransformExitLeft(popUpTransform, ()=>
            {
                callBack?.Invoke();
            });
        }
        
        private void UnloadSelf()
        {
            AssetReferenceLoader.DestroyOrUnload(gameObject);
            Destroy(gameObject);
        }
    }
}