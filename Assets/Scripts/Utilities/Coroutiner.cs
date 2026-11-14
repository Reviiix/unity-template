using UnityEngine;
using System.Collections;

namespace Utilities
{
    /// <summary>
    /// Convenience class that creates an ephemeral MonoBehaviour instance.
    /// Through which static classes can call StartCoroutine.
    /// </summary>
    public static class Coroutiner
    {
        private static bool _initialised;
        private static CoroutinerInstance _routineHandler;
        
        /// <summary>
        /// Create GameObject with MonoBehaviour to handle task (If non exists) then cache it.
        /// Use cached MonoBehaviour instance to run coroutine on demand.
        /// </summary>
        /// <param name="iterationResult"> The coroutine to be ran.</param>
        /// <returns></returns>
        public static Response StartCoroutine(IEnumerator iterationResult)
        {
            if (!_initialised)
            {
                CreateCoroutineHandler();
            }

            var response = new Response();
            var routine = _routineHandler.ProcessWork(iterationResult, response);
            response.Coroutine = routine;
            return response;
        }
        
        public static void StopCoroutine(Coroutine routine)
        {
            _routineHandler.StopCoroutine(routine);
        }

        private static void CreateCoroutineHandler()
        {
            var routineHandlerGameObject = new GameObject(nameof(Coroutiner));
            _routineHandler = routineHandlerGameObject.AddComponent(typeof(CoroutinerInstance)) as CoroutinerInstance;
            _initialised = true;
        }

        private class CoroutinerInstance : MonoBehaviour
        {
            private Response responseCache;
        
            private void Awake()
            {
                DontDestroyOnLoad(this);
            }
    
            public Coroutine ProcessWork(IEnumerator iterationResult, Response response)
            {
                responseCache = response;
                return StartCoroutine(CompleteWork(iterationResult));
            }
    
            private IEnumerator CompleteWork(IEnumerator iterationResult)
            {
                yield return StartCoroutine(iterationResult);
                yield return null;
                responseCache.IsComplete = true;
            }
        }
        
        public class Response
        {
            public Coroutine Coroutine;
            public bool IsComplete;
        }
    }
}