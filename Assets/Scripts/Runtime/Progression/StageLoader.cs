using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityTemplate.UserInterface;

namespace UnityTemplate.Progression
{
    /// <summary>Loads a stage scene behind a one-second fade to black and back, ignoring further requests while a load is running.</summary>
    public class StageLoader
    {
        private const float FadeSeconds = 1;
        private readonly StageCatalogue catalogue;
        private readonly UserInterfaceManager userInterface;

        public bool IsLoading { get; private set; }

        /// <summary>Creates the loader for a stage catalogue and the user interface that owns the fade.</summary>
        public StageLoader(StageCatalogue catalogue, UserInterfaceManager userInterface)
        {
            this.catalogue = catalogue;
            this.userInterface = userInterface;
        }

        /// <summary>Returns true when a stage load can start now: the stage has a scene, no load is running and the start-up popups are done.</summary>
        public bool CanLoadStage(int group, int stage)
        {
            return !IsLoading && !userInterface.IsShowingStartUpPopUps && catalogue.HasScene(group, stage);
        }

        /// <summary>Fades out, hides the menus, loads the stage's scene and fades back in.</summary>
        public async void LoadStage(int group, int stage)
        {
            if (!CanLoadStage(group, stage)) return;

            IsLoading = true;
            try
            {
                await userInterface.FadeScreenAsync(1, FadeSeconds);
                userInterface.HideMenusForStage();
                await Addressables.LoadSceneAsync(catalogue.GetScene(group, stage)).Task;
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
    }
}
