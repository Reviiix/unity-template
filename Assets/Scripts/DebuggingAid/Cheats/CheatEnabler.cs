using UnityEngine;
using UnityEngine.UI;

namespace DebuggingAid.Cheats
{
    [RequireComponent(typeof(Canvas))]
    public class CheatEnabler : MonoBehaviour 
    {
        private static Canvas _cheatMenu;
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
            Destroy(gameObject);
        }
    }
}
