using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = System.Object;

public static class AssetReferenceLoader 
{
    public static void LoadScene(IKeyEvaluator scene, Action callBack = null)
    {
        Addressables.LoadSceneAsync(scene).Completed += (loadComplete)=>
        {
            callBack?.Invoke();
        };
    }
    
    public static void LoadAssetReferenceAsynchronously<T>(IKeyEvaluator assetReference, Action<T> callBack)
    {
        //if (!assetReference.RuntimeKeyIsValid()) return; //TODO
        
        var operationHandler = Addressables.LoadAssetAsync<T>(assetReference);

        operationHandler.Completed += (operation) =>
        {
            callBack(operation.Result);
            Addressables.Release(operationHandler);
        };
    }
    
    public static void DestroyOrUnload(GameObject gameObject)
    {
        Addressables.ReleaseInstance(gameObject);
    }
    
    public static void UnloadAssetReference(AssetReference reference)
    {
        reference.ReleaseAsset();
    }
    
    public static void UnloadAssetReferenceInstance(AssetReference reference, GameObject gameObjectReference)
    {
        reference.ReleaseInstance(gameObjectReference);
    }
}
