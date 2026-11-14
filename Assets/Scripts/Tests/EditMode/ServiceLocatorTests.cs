using System;
using NUnit.Framework;
using UnityTemplate.Core;

namespace UnityTemplate.Tests
{
    /// <summary>Tests registering, resolving and resetting services.</summary>
    public class ServiceLocatorTests
    {
        [TearDown]
        public void Reset()
        {
            ServiceLocator.Reset();
        }

        [Test]
        public void ARegisteredServiceCanBeResolved()
        {
            var service = new FakeService();
            ServiceLocator.Register(service);

            Assert.AreSame(service, ServiceLocator.Get<FakeService>());
            Assert.IsTrue(ServiceLocator.TryGet<FakeService>(out _));
        }

        [Test]
        public void ResolvingAMissingServiceExplainsWhatToDo()
        {
            var error = Assert.Throws<InvalidOperationException>(() => ServiceLocator.Get<FakeService>());
            StringAssert.Contains("Initialisation.unity", error.Message);
        }

        [Test]
        public void RegisteringTheSameTypeTwiceIsRejected()
        {
            ServiceLocator.Register(new FakeService());

            Assert.Throws<InvalidOperationException>(() => ServiceLocator.Register(new FakeService()));
        }

        private class FakeService
        {
        }
    }
}
