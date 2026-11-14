using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTemplate.Core
{
    /// <summary>Global registry of game services: GameInitialiser registers each service once and any script resolves it with Get.</summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        /// <summary>Registers a service under its type; registering the same type twice is a programming error.</summary>
        public static void Register<T>(T service) where T : class
        {
            if (service == null) throw new ArgumentNullException(nameof(service));

            var type = typeof(T);
            if (Services.ContainsKey(type))
            {
                throw new InvalidOperationException($"A {type.Name} is already registered. Each service is registered once, by GameInitialiser.");
            }
            Services.Add(type, service);
        }

        /// <summary>Returns the registered service, or throws a descriptive error when it has not been registered yet.</summary>
        public static T Get<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out var service)) return (T)service;

            throw new InvalidOperationException($"No {typeof(T).Name} is registered. Resolve services after GameInitialiser has initialised them (press Play in Initialisation.unity, which runs GameInitialiser).");
        }

        /// <summary>Returns true and the service when it is registered, without throwing.</summary>
        public static bool TryGet<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var registered))
            {
                service = (T)registered;
                return true;
            }
            service = null;
            return false;
        }

        /// <summary>Removes a service, for example when the object that provides it is destroyed.</summary>
        public static void Unregister<T>() where T : class
        {
            Services.Remove(typeof(T));
        }

        /// <summary>Removes every service; runs before each play session (so disabling domain reload is safe) and when the boot objects are destroyed.</summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            Services.Clear();
        }
    }
}
