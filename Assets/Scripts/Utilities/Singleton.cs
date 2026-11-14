using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Inheriting from this class will follow the singleton pattern.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected virtual void OnEnable()
        {
            if (Instance != null)
            {
                Debug.LogError(DebuggingAid.DebugLogManager.DuplicateErrorMessagePrefix + name + DebuggingAid.DebugLogManager.DuplicateSingletonErrorMessageSuffix + gameObject + DebuggingAid.DebugLogManager.FullStop);
                AssetReferenceLoader.DestroyOrUnload(gameObject);
                return;
            }
            
            Instance = FindFirstObjectByType(typeof(T)) as T;
            if (Instance == null)
            {
                Debug.LogError(DebuggingAid.DebugLogManager.CanNotFindSingletonErrorMessage + name + DebuggingAid.DebugLogManager.FullStop);
            }
        }

        protected virtual void OnDisable()
        {
            Instance = null;
        }
    }
    
    public abstract class PrivateSingleton<T> : MonoBehaviour where T : class
    {
        protected static T Instance;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError(DebuggingAid.DebugLogManager.DuplicateErrorMessagePrefix + name + DebuggingAid.DebugLogManager.DuplicateSingletonErrorMessageSuffix + gameObject + DebuggingAid.DebugLogManager.FullStop);
                AssetReferenceLoader.DestroyOrUnload(gameObject);
                return;
            }
            
            Instance = FindFirstObjectByType(typeof(T)) as T;
            if (Instance == null)
            {
                Debug.LogError(DebuggingAid.DebugLogManager.CanNotFindSingletonErrorMessage + name + DebuggingAid.DebugLogManager.FullStop);
            }
        }

        protected virtual void OnDisable()
        {
            Instance = null;
        }
    }
}