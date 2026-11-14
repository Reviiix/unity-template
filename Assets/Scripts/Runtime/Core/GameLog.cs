using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace UnityTemplate.Core
{
    /// <summary>Development-only logging: calls are compiled out of release builds, so their messages are never built there.</summary>
    public static class GameLog
    {
        /// <summary>Logs an informational message in the Editor and development builds only.</summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        /// <summary>Logs a warning in the Editor and development builds only.</summary>
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Warning(string message)
        {
            Debug.LogWarning(message);
        }
    }
}
