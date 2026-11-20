using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityTemplate.Utilities
{
    /// <summary>Awaits Addressables operations through their Completed callback, which works on every platform; handle.Task isn't supported on WebGL.</summary>
    public static class AsyncOperationHandleExtensions
    {
        /// <summary>Waits for the operation to finish and returns its result, or default if it failed (check the handle's Status).</summary>
        public static async Awaitable<T> ToAwaitable<T>(this AsyncOperationHandle<T> handle)
        {
            if (!handle.IsDone)
            {
                var completion = new AwaitableCompletionSource();
                handle.Completed += _ => completion.TrySetResult();
                await completion.Awaitable;
            }
            return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : default;
        }
    }
}
