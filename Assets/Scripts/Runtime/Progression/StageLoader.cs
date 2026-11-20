using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityTemplate.UserInterface;
using UnityTemplate.Utilities;

namespace UnityTemplate.Progression
{
    /// <summary>Loads a stage scene on top of the boot scene behind a one-second fade, and unloads it again to return to the main menu; ignores requests while either is running.</summary>
    public class StageLoader
    {
        private const float FadeSeconds = 1;
        private readonly StageCatalogue catalogue;
        private readonly UserInterfaceManager userInterface;
        private readonly Camera menuCamera;
        private AsyncOperationHandle<SceneInstance> stageHandle;
        private Scene menuScene;

        public bool IsLoading { get; private set; }
        public bool IsInStage => stageHandle.IsValid();

        /// <summary>Creates the loader for a stage catalogue, the user interface that owns the fade, and the boot scene's camera, which stages replace with their own.</summary>
        public StageLoader(StageCatalogue catalogue, UserInterfaceManager userInterface, Camera menuCamera)
        {
            this.catalogue = catalogue;
            this.userInterface = userInterface;
            this.menuCamera = menuCamera;
        }

        /// <summary>Returns true when a stage load can start now: the stage has a scene, no stage is loading or open and the start-up popups are done.</summary>
        public bool CanLoadStage(int group, int stage)
        {
            return !IsLoading && !IsInStage && !userInterface.IsShowingStartUpPopUps && catalogue.HasScene(group, stage);
        }

        /// <summary>Fades out, hides the menus, loads the stage's scene additively, makes it the active scene and fades back in.</summary>
        public async void LoadStage(int group, int stage)
        {
            if (!CanLoadStage(group, stage)) return;

            IsLoading = true;
            try
            {
                await userInterface.FadeScreenAsync(1, FadeSeconds);
                menuScene = SceneManager.GetActiveScene();
                userInterface.HideMenusForStage();
                stageHandle = Addressables.LoadSceneAsync(catalogue.GetScene(group, stage), LoadSceneMode.Additive);
                var stageInstance = await stageHandle.ToAwaitable();
                if (stageHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    SceneManager.SetActiveScene(stageInstance.Scene);
                    SetMenuCameraEnabled(false);
                }
                else
                {
                    Debug.LogError($"Stage {group}-{stage} could not be loaded. {stageHandle.OperationException}");
                    Addressables.Release(stageHandle);
                    stageHandle = default;
                    userInterface.ShowMenusAfterStage();
                }
                await userInterface.FadeScreenAsync(0, FadeSeconds);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>Fades out, unloads the open stage, shows the main menu again and fades back in; does nothing outside a stage.</summary>
        public async void ReturnToMainMenu()
        {
            if (IsLoading || !IsInStage) return;

            IsLoading = true;
            try
            {
                await userInterface.FadeScreenAsync(1, FadeSeconds);
                await Addressables.UnloadSceneAsync(stageHandle).ToAwaitable();
                stageHandle = default;
                if (menuScene.IsValid() && menuScene.isLoaded) SceneManager.SetActiveScene(menuScene);
                SetMenuCameraEnabled(true);
                userInterface.ShowMenusAfterStage();
                await userInterface.FadeScreenAsync(0, FadeSeconds);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>Switches the boot scene's camera, which only draws the menus' backdrop colour, on or off.</summary>
        private void SetMenuCameraEnabled(bool enabled)
        {
            if (menuCamera != null) menuCamera.enabled = enabled;
        }
    }
}
