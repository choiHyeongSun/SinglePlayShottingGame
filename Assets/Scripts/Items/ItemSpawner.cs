using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public enum LabelType
{
    Items,
    WeaponItems,
    ConsumableItems
}
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private LabelType labelType;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private ItemUI itemInfoUI;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private bool isRandom;
    public ItemObject item { get; set; }
    private void Start()
    {
        if (isRandom == false) return;

        Addressables.LoadResourceLocationsAsync(labelType.ToString()).Completed += (handle) =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }

            var items = handle.Result;
            int getItem = Random.Range(0, items.Count);
            Addressables.InstantiateAsync(items[getItem], spawnPoint).Completed += (handle) =>
            {
                if (handle.Status != AsyncOperationStatus.Succeeded)
                {
                    return;
                }
                item = handle.Result.GetComponent<ItemObject>();
                itemInfoUI.itemInfo = item.itemInfo;

                if (labelType == LabelType.ConsumableItems)
                {
                    item.count = Random.Range(1, 10);
                }
            };
            Addressables.Release(handle);
        };
    }

    public void SetItem(List<String> labelList, int count)
    {
        var locationHandel = Addressables.LoadResourceLocationsAsync(labelList, Addressables.MergeMode.Intersection);
        locationHandel.WaitForCompletion();
        if (locationHandel.Status != AsyncOperationStatus.Succeeded)
        {
            return;
        }

        var items = locationHandel.Result[0];
        var itemhandle = Addressables.InstantiateAsync(items, spawnPoint);
        itemhandle.WaitForCompletion();
        if (itemhandle.Status != AsyncOperationStatus.Succeeded)
        {
            return;
        }
        item = itemhandle.Result.GetComponent<ItemObject>();
        item.count = count;
        itemInfoUI.itemInfo = item.itemInfo;

        Addressables.Release(locationHandel);
    }
    private void Update()
    {
        float yaw = spawnPoint.eulerAngles.y + rotateSpeed * Time.deltaTime;
        Vector3 newEulerRotate = new Vector3(0.0f, yaw, 0.0f);
        spawnPoint.eulerAngles = newEulerRotate;
    }

    private void OnTriggerEnter(Collider other)
    {
        itemInfoUI.gameObject.SetActive(true);
        Player player = other.GetComponent<Player>();
        player.playerItemController.AddItemObject(this);
    }

    private void OnTriggerExit(Collider other)
    {
        itemInfoUI.gameObject.SetActive(false);
        Player player = other.GetComponent<Player>();
        player.playerItemController.RemoveItemObject(this);
    }
}
