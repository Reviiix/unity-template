using UnityEngine;

namespace UnityTemplate.Debugging
{
    /// <summary>Activates the cheat menu in the Editor and development builds and deletes it from release builds.</summary>
    public class CheatEnabler : MonoBehaviour
    {
        [SerializeField] private GameObject cheats;

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            cheats.SetActive(true);
#else
            Destroy(cheats);
#endif
            Destroy(this);
        }
    }
}
