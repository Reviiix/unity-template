using UnityEngine;

namespace MostlyPureFunctions
{
    public class PersistObjects : MonoBehaviour
    {
        [SerializeField] private Transform[] persistantObjects;
    
        private void Awake()
        {
            SetObjectsToPersistAcrossScenes();
        }

        private void SetObjectsToPersistAcrossScenes()
        {
            foreach (var objectToPersist in persistantObjects)
            {
                DontDestroyOnLoad(objectToPersist);
            }
            Destroy(GetComponent<PersistObjects>());
        }
    }
}
