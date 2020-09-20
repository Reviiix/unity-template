using UnityEngine;

namespace PureFunctions
{
    //This class will check the ProjectSettings class to see if it has already persisted these objects.
    //If it has: It will destroy the objects and itself (because they already exists).
    //If it has not: It will persist the object and destroy itself when the job is done.
    public class PersistObjects : MonoBehaviour
    {
        private static bool FirstLoad => !ProjectSettings.PersistantObjectsInitialisedPreviously;
        [SerializeField]
        private Transform[] persistantObjects;
    
        private void Awake()
        {
            SetInstance();
            SetObjectsToPersistAcrossScenes();
        }
    
        private void SetInstance()
        {
            if (!ProjectSettings.PersistantObjectsInitialisedPreviously)
            {
                return;
            }
            DestroyAllObjects();
        }

        private void SetObjectsToPersistAcrossScenes()
        {
            if (!FirstLoad) return;
            
            foreach (var objectToPersist in persistantObjects)
            {
                DontDestroyOnLoad(objectToPersist);
            }
            ProjectSettings.PersistantObjectsInitialisedPreviously = true;
            Destroy(GetComponent<PersistObjects>());
        }
        
        private void DestroyAllObjects()
        {
            foreach (var objectToPersist in persistantObjects)
            {
                Destroy(objectToPersist.gameObject);
            }
        }
    }
}
