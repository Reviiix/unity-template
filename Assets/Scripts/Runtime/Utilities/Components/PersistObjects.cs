using UnityEngine;
using UnityEngine.Serialization;

namespace UnityTemplate.Utilities
{
    /// <summary>Keeps the listed root objects alive across stage scene loads, then removes itself.</summary>
    public class PersistObjects : MonoBehaviour
    {
        [FormerlySerializedAs("persistantObjects")] [SerializeField] private Transform[] persistentObjects;

        /// <summary>Marks every listed object DontDestroyOnLoad and destroys this component.</summary>
        private void Awake()
        {
            foreach (var objectToPersist in persistentObjects)
            {
                DontDestroyOnLoad(objectToPersist);
            }
            Destroy(this);
        }
    }
}
