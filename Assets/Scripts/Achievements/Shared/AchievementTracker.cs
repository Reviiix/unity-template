using System.Collections;
using UnityEngine;

public abstract class AchievementTracker : MonoBehaviour
{
    private Coroutine _intermittentChecks;
    private static readonly WaitForSeconds TenMinutes = new WaitForSeconds(600);
    protected static readonly WaitForSeconds OneHour = new WaitForSeconds(3600);
    
    protected virtual void OnEnable()
    {
        _intermittentChecks = StartCoroutine(PerformChecksIntermittently());
    }
    
    protected virtual void OnDisable()
    {
        if (_intermittentChecks != null)
        {
            StopCoroutine(_intermittentChecks);
        }
    }
    
    // ReSharper disable once FunctionRecursiveOnAllPaths
    private IEnumerator PerformChecksIntermittently()
    {
        PerformChecks();
        yield return TenMinutes;
        _intermittentChecks = StartCoroutine(PerformChecksIntermittently());
    }
    
    protected virtual void PerformChecks()
    {
        Debug.LogWarning("Override this method!");
    }
}
