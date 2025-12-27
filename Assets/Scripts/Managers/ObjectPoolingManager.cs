using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolingManager : MonoBehaviour
{
    private Dictionary<String, AssetReference> poolAssetReferences = new();
    private Dictionary<String, Queue<GameObject>> poolObjects = new ();

    private List<GameObject> AllPoolObjects = new();
    private readonly int createObjects = 10;

    public void RegisterPooling(AssetReference assetReference)
    {
        poolAssetReferences.TryAdd(assetReference.AssetGUID, assetReference);
        poolObjects.TryAdd(assetReference.AssetGUID, new Queue<GameObject>());
    }

    public GameObject PopObject(AssetReference assetReference)
    {
        String guid = assetReference.AssetGUID;
        if (!poolAssetReferences.ContainsKey(guid))
        {
            Debug.LogError("AssetReference가 등록되지 않았습니다.");
            return null;
        }

        if (poolObjects[guid].Count == 0)
        {
            CreateObjectPool(assetReference);
        }

        GameObject obj = poolObjects[guid].Dequeue();
        obj.SetActive(true);
        return obj;
    }
    public void PushObject(AssetReference assetReference, GameObject obj)
    {
        String guid = assetReference.AssetGUID;
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        poolObjects[guid].Enqueue(obj);
    }

    private void CreateObjectPool(AssetReference assetReference)
    {
        String guid = assetReference.AssetGUID;
        List<AsyncOperationHandle<GameObject>> handles = new();
        for (int i = 0; i < createObjects; i++)
        {
            handles.Add(Addressables.InstantiateAsync(poolAssetReferences[guid]));
        }

        foreach (var handle in handles)
        {
            handle.WaitForCompletion();
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                continue;
            }
            GameObject obj = handle.WaitForCompletion();
            PoolObject poolObject = obj.AddComponent<PoolObject>();
            poolObject.originReference = assetReference;
            AllPoolObjects.Add(obj);
            poolObjects[guid].Enqueue(obj);
            obj.SetActive(false);
        }
    }
    public void CleanPool()
    {
        foreach (var obj in AllPoolObjects)
        {
            Addressables.ReleaseInstance(obj);
        }
    }
}
