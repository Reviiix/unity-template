using System.Collections;
using UnityEngine;

namespace Achievements.Shared
{
    /// <summary>
    /// This class is the base for achievement tracking. it is shared by dynamic and permanent achievements.
    /// </summary>
    public abstract class AchievementTracker : MonoBehaviour
    {
        private Coroutine intermittentChecks;
        private static readonly WaitForSeconds WaitTenMinutes = new (600);
        protected static readonly WaitForSeconds WaitOneHour = new (3600);
    
        protected virtual void OnEnable()
        {
            if (!ProjectManager.EnabledFeatures.Achievements)
            {
                Destroy(this);
                return;
            }
            intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }
    
        protected virtual void OnDisable()
        {
            if (intermittentChecks != null)
            {
                StopCoroutine(intermittentChecks);
            }
        }
    
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator PerformChecksIntermittently()
        {
            PerformChecks();
            yield return WaitTenMinutes;
            intermittentChecks = StartCoroutine(PerformChecksIntermittently());
        }
    
        protected virtual void PerformChecks()
        {
            const string errorMessage = "Override this method with the Checks needed to track achievements!";
            Debug.LogWarning(errorMessage);
        }
    }
}
