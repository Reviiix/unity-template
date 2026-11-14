using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// This class sets all the objects in its array to not destroy on load
    /// The class will then remove itself as its no longer needed
    /// </summary>
    public class PersistObjects : MonoBehaviour
    {
        [SerializeField] private Transform[] persistantObjects;
    
        private void Awake()
        {
            SetObjectsToPersistAcrossScenes();
            Destroy(this);
        }

        private void SetObjectsToPersistAcrossScenes()
        {
            foreach (var objectToPersist in persistantObjects)
            {
                DontDestroyOnLoad(objectToPersist);
            }
        }
    }
}
