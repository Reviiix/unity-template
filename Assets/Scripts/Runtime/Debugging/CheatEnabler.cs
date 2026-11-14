using UnityEngine;

namespace UnityTemplate.Debugging
{
    /// <summary>Shows the cheat system in the Editor and development builds and deletes it from release builds.</summary>
    public class CheatEnabler : MonoBehaviour
    {
        [SerializeField] private GameObject cheats;

        /// <summary>Activates the cheat page in debug builds, or destroys the whole cheat system (including its button) in release builds.</summary>
        private void Awake()
        {
            if (!Debug.isDebugBuild)
            {
                Destroy(gameObject);
                return;
            }

            cheats.SetActive(true);
            Destroy(this);
        }
    }
}
