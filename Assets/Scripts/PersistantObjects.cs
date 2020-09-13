using UnityEngine;
using UnityEngine.Serialization;

public class PersistantObjects : MonoBehaviour
{
    [FormerlySerializedAs("persistentObjects")] public Transform[] objectsToPersist;
    
    private void Awake()
    {
        foreach (var v in objectsToPersist)
        {
            DontDestroyOnLoad(v);
        }
    }
}
