using System;
using System.Collections;
using System.Collections.Generic;
using Abstract;
using UnityEngine;
using UnityEngine.AddressableAssets;

//Objects that are created as part of the game are pooled by this glass and enabled as they are needed.
//This system produces less garbage than instantiating and destroying objects.

//Game objects are created from addressable assets meaning they are loaded into memory as they are needed then need to be unloaded when they are not.
//Pools are created by copying one instance of a converted addressable asset.
public class ObjectPooling : PrivateSingleton<ObjectPooling>
{
    [SerializeField] private Pool[] pools;
    private static readonly Dictionary<int, Queue<GameObject>> PoolDictionary = new Dictionary<int, Queue<GameObject>>();
    
    private static readonly WaitUntil WaitUntilAssetReferenceIsLoadedAsynchronously = new WaitUntil(() => _currentAddressableAsGameObject != null);
    private static GameObject _currentAddressableAsGameObject;

    public void Initialise(Action callBack)
    {
        StartCoroutine(ConvertPoolListIntoQueueOfGameObjects(callBack));
    }

    public static GameObject ReturnObjectFromPool(int index, Vector3 position, Quaternion rotation, bool setActive = true)
    {
        var returnObject = PoolDictionary[index].Dequeue();
        
        returnObject.transform.localPosition = position;
        returnObject.transform.rotation = rotation;
        returnObject.SetActive(setActive);
            
        PoolDictionary[index].Enqueue(returnObject);
        
        return returnObject;
    }

    public static int ReturnMaximumActiveObjects(int poolIndex)
    {
        return PoolDictionary[poolIndex].Count;
    }
    
    private IEnumerator ConvertPoolListIntoQueueOfGameObjects(Action callBack)
    {
        var poolsCache = pools;
        foreach (var pool in poolsCache)
        {
            var objectPool = new Queue<GameObject>();
            var maximumAmountOfObjectsInCurrentPool = pool.maximumActiveObjects;
            var assetReference = pool.assetReference;
            
            AssetReferenceLoader.LoadAssetReferenceAsynchronously<GameObject>(assetReference, (returnVariable) =>
            {
                _currentAddressableAsGameObject = returnVariable;
            });
            
            yield return WaitUntilAssetReferenceIsLoadedAsynchronously;
            
            for (var i = 0; i < maximumAmountOfObjectsInCurrentPool; i++)
            {
                var gameObjectCache = Instantiate(_currentAddressableAsGameObject);
                gameObjectCache.SetActive(false);
                objectPool.Enqueue(gameObjectCache);
            }
            
            AssetReferenceLoader.UnloadGAmeObjectAssetReference(_currentAddressableAsGameObject);
            _currentAddressableAsGameObject = null;

            if (PoolDictionary.ContainsKey(pool.index))
            {
                DebuggingAid.Debugging.DisplayDebugMessage("Replacing object pool " + pool.index + ". ");
                PoolDictionary.Remove(pool.index);
            }
            
            PoolDictionary.Add(pool.index, objectPool);
        }

        pools = null;
        callBack();
    }
}
    
[Serializable]
public struct Pool
{
    public int index;
    public AssetReference assetReference;
    public int maximumActiveObjects;
}