using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace DebuggingAid.Cheats
{
    /// <summary>
    /// This class will enable / disable the cheat functionality.
    /// </summary>
    [RequireComponent(typeof(GameObject))]
    public class CheatEnabler : MonoBehaviour 
    {
        [SerializeField] private GameObject cheats;

        private void Awake()
        {
            Validate();
        }
        
        private void Validate()
        {
            #if UNITY_EDITOR || DEBUG_BUILD
            cheats.SetActive(true);
            Destroy(this);
            return;
            #endif
            AssetReferenceLoader.DestroyOrUnload(gameObject);
        }
    }
}
