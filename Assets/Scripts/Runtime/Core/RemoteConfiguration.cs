using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.RemoteConfig;
using UnityEngine;

namespace UnityTemplate.Core
{
    /// <summary>Reads the game configuration Remote Config cached on the last launch, and refreshes that cache in the background for the next launch.</summary>
    public static class RemoteConfiguration
    {
        private const string ConfigType = "settings";
        private const string ConfigurationKey = "Configuration";

        /// <summary>Returns the cached remote configuration laid over the built-in defaults; invalid or missing values keep their defaults.</summary>
        public static GameConfiguration LoadCachedConfiguration()
        {
            var configuration = new GameConfiguration();
            try
            {
                var cachedJson = RemoteConfigService.Instance.GetConfig(ConfigType).GetJson(ConfigurationKey, "{}");
                JsonUtility.FromJsonOverwrite(cachedJson, configuration);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Cached Remote Config could not be read; using the built-in configuration. " + exception.Message);
            }
            return configuration.IsValid ? configuration : new GameConfiguration();
        }

        /// <summary>Signs in anonymously and fetches the latest configuration, which Remote Config caches for the next launch; never throws.</summary>
        public static async void FetchForNextLaunchAsync()
        {
            try
            {
                if (UnityServices.State == ServicesInitializationState.Uninitialized) await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn) await AuthenticationService.Instance.SignInAnonymouslyAsync();
                await RemoteConfigService.Instance.FetchConfigsAsync(new UserAttributes(), new AppAttributes());
                GameLog.Info("Remote Config fetched; it applies from the next launch.");
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Remote Config fetch failed (offline or project not linked to Unity Cloud): " + exception.Message);
            }
        }

        /// <summary>Empty user attributes, which Remote Config requires.</summary>
        private struct UserAttributes { }

        /// <summary>Empty app attributes, which Remote Config requires.</summary>
        private struct AppAttributes { }
    }
}
