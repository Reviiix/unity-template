using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace DebuggingAid.Cheats
{
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
            #if UNITY_EDITOR || DEBUGBUILD
            cheats.SetActive(true);
            Destroy(GetComponent<CheatEnabler>());
            return;
            #endif
            AssetReferenceLoader.DestroyOrUnload(gameObject);
        }
    }
}
