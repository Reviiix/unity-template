using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is caches the main camera to avoid having to use expensive Camera.main
/// </summary>
public static class CameraManager
{
    public static Camera ActiveCamera { get; private set; }
    public static int ReturnScreenWidth => ActiveCamera.pixelWidth;
    
    public static void Initialise()
    {
        SceneManager.sceneLoaded += OnSceneChange;
    }

    private static void OnSceneChange(Scene scene, LoadSceneMode loadMode)
    {
        SetActiveCamera();
    }
    
    private static void SetActiveCamera()
    {
        ActiveCamera = Camera.main; //Camera.main is an expensive invocation, only call when the camera changes (i.e scene changes)
    }
}
