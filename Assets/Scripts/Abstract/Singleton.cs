using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Abstract
{
    public abstract class Singleton<T> : MonoBehaviour where T : class
    {
        public static T Instance { get; private set; }

        protected virtual void OnEnable()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There are 2 " + name + " singletons in the scene. Removing " + gameObject + ".");
                AssetReferenceLoader.DestroyOrUnload(gameObject);
                return;
            }
            
            Instance = FindObjectOfType(typeof(T)) as T;
            if (Instance == null) Debug.LogError("Can not find singleton: " + name + ".");
        }

        protected virtual void OnDisable()
        {
            Instance = null;
        }
    }
    
    public abstract class PrivateSingleton<T> : MonoBehaviour where T : class
    {
        protected static T Instance;

        protected virtual void OnEnable()
        {
            if (Instance != null)
            {
                Debug.LogWarning("There are 2 " + name + " singletons in the scene. Removing " + gameObject + ".");
                AssetReferenceLoader.DestroyOrUnload(gameObject);
                return;
            }
            
            Instance = FindObjectOfType(typeof(T)) as T;
            if (Instance == null) Debug.LogError("Can not find singleton: " + name + ".");
        }

        protected virtual void OnDisable()
        {
            Instance = null;
        }
    }
}
