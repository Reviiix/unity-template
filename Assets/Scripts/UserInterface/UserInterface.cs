using System;
using JetBrains.Annotations;
using PureFunctions.Movement;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    [RequireComponent(typeof(Canvas))]
    public abstract class PopUpInterface : MonoBehaviour
    {
        protected Canvas Display;
        [SerializeField] protected Transform popUpMenu;
        [FormerlySerializedAs("closeMenu")] [SerializeField] [CanBeNull] protected Button closeButton;
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
        
        protected void UnloadSelf()
        {
            AssetReferenceLoader.DestroyOrUnload(gameObject);
            Destroy(gameObject);
        }
    }
}