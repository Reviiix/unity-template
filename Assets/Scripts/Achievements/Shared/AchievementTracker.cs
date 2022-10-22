using System.Collections;
using UnityEngine;

/// <summary>
/// This class is the base for achievement tracking. it is shared by dynamic and permanent achievements.
/// </summary>
public abstract class AchievementTracker : MonoBehaviour
{
    private Coroutine intermittentChecks;
    private static readonly WaitForSeconds WaitTenMinutes = new WaitForSeconds(600);
    protected static readonly WaitForSeconds WaitOneHour = new WaitForSeconds(3600);
    
    protected virtual void OnEnable()
    {
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
        Debug.LogWarning("Override this method!");
    }
}
