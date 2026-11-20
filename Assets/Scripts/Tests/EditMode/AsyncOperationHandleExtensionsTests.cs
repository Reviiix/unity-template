using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;
using UnityTemplate.Utilities;

namespace UnityTemplate.Tests
{
    /// <summary>Checks that ToAwaitable waits for an Addressables operation through its Completed callback and returns its result.</summary>
    public class AsyncOperationHandleExtensionsTests
    {
        private ResourceManager resourceManager;
        private Action<AsyncOperationHandle, Exception> previousExceptionHandler;

        /// <summary>An operation that finishes only when the test says so.</summary>
        private sealed class ManualOperation : AsyncOperationBase<string>
        {
            protected override void Execute() { }

            public void Finish(string result) => Complete(result, true, string.Empty);
        }

        [SetUp]
        public void CreateResourceManager()
        {
            resourceManager = new ResourceManager();
            previousExceptionHandler = ResourceManager.ExceptionHandler;
            ResourceManager.ExceptionHandler = (_, _) => { };
        }

        [TearDown]
        public void DisposeResourceManager()
        {
            ResourceManager.ExceptionHandler = previousExceptionHandler;
            resourceManager.Dispose();
        }

        [Test]
        public void ACompletedOperationReturnsItsResultAtOnce()
        {
            var handle = resourceManager.CreateCompletedOperation("done", null);
            var awaiter = handle.ToAwaitable().GetAwaiter();
            Assert.IsTrue(awaiter.IsCompleted);
            Assert.AreEqual("done", awaiter.GetResult());
            handle.Release();
        }

        [UnityTest]
        public IEnumerator APendingOperationReturnsItsResultWhenItCompletes()
        {
            var operation = new ManualOperation();
            var handle = resourceManager.StartOperation(operation, default);
            var awaiter = handle.ToAwaitable().GetAwaiter();
            yield return null;
            Assert.IsFalse(awaiter.IsCompleted, "Finished before the operation completed.");

            operation.Finish("loaded");
            for (var frame = 0; frame < 10 && !awaiter.IsCompleted; frame++) yield return null;
            Assert.IsTrue(awaiter.IsCompleted, "Didn't finish after the operation completed.");
            Assert.AreEqual("loaded", awaiter.GetResult());
            handle.Release();
        }

        [Test]
        public void AFailedOperationReturnsDefault()
        {
            var handle = resourceManager.CreateCompletedOperation<string>(null, "Simulated failure");
            var awaiter = handle.ToAwaitable().GetAwaiter();
            Assert.IsTrue(awaiter.IsCompleted);
            Assert.IsNull(awaiter.GetResult());
            Assert.AreEqual(AsyncOperationStatus.Failed, handle.Status);
            handle.Release();
        }
    }
}
