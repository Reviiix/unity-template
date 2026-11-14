using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace UnityTemplate.Core
{
    /// <summary>Pre-instantiates every pooled prefab at start-up and hands instances out and back, so nothing is instantiated or destroyed during play.</summary>
    public class ObjectPooler : MonoBehaviour
    {
        [SerializeField] private Pool[] pools = Array.Empty<Pool>();
        private readonly Dictionary<PoolKey, ObjectPool<GameObject>> objectPools = new();
        private readonly Dictionary<PoolKey, int> poolSizes = new();

        /// <summary>Creates each pool and instantiates its instances up front (inactive, parented to the pooler); called once by GameInitialiser.</summary>
        public void Initialise()
        {
            foreach (var pool in pools)
            {
                var prefab = pool.prefab;
                var objectPool = new ObjectPool<GameObject>(
                    () => CreateInstance(prefab),
                    actionOnRelease: ResetInstance,
                    actionOnDestroy: Destroy,
                    defaultCapacity: pool.size,
                    maxSize: Mathf.Max(1, pool.size) * 4);

                Prewarm(objectPool, pool.size);
                objectPools[pool.key] = objectPool;
                poolSizes[pool.key] = pool.size;
            }
        }

        /// <summary>Returns how many instances a pool was created with.</summary>
        public int GetPoolSize(PoolKey key)
        {
            return poolSizes.TryGetValue(key, out var size) ? size : 0;
        }

        /// <summary>Takes an instance from a pool (an extra one is created if the pool is empty) and optionally activates it.</summary>
        public GameObject Get(PoolKey key, bool activate = true)
        {
            if (!objectPools.TryGetValue(key, out var objectPool))
            {
                throw new InvalidOperationException($"Pool {key} is not set up on the ObjectPooler in Initialisation.unity.");
            }

            var instance = objectPool.Get();
            instance.SetActive(activate);
            return instance;
        }

        /// <summary>Deactivates an instance and returns it to its pool for reuse.</summary>
        public void Release(PoolKey key, GameObject instance)
        {
            objectPools[key].Release(instance);
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            var instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            return instance;
        }

        private void ResetInstance(GameObject instance)
        {
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
        }

        private static void Prewarm(ObjectPool<GameObject> objectPool, int amount)
        {
            var instances = new GameObject[amount];
            for (var i = 0; i < amount; i++) instances[i] = objectPool.Get();
            foreach (var instance in instances) objectPool.Release(instance);
        }

        [Serializable]
        private struct Pool
        {
            [FormerlySerializedAs("index")] public PoolKey key;
            public GameObject prefab;
            [FormerlySerializedAs("maximumActiveObjects")] public int size;
        }
    }

    /// <summary>Identifies each object pool; values are stored as numbers in the scene, so only ever append new entries.</summary>
    public enum PoolKey
    {
        AudioSource = 0,
        AchievementPopUp = 1,
    }
}
