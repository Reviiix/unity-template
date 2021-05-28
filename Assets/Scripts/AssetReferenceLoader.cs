using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
    
    public static void UnloadGAmeObjectAssetReference(GameObject gameObjectReference)
    {
        Addressables.ReleaseInstance(gameObjectReference);
    }
}
