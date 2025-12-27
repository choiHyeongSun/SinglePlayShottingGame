using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] private GameObject WeaponSpawner;
    private Player player;
    private ItemInfo prevItemInfo;
    private List<ItemSpawner> itemIntersaction = new();
    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Start()
    {
        PlayerInput playerInput = player.playerInputController.playerInput;
        playerInput.Player.OnItemPickup.performed += ItemPickUp;
        playerInput.Player.OnNumberKey.performed += OnNumberKey;
    }

    public void AddItemObject(ItemSpawner itemObject)
    {
        itemIntersaction.Add(itemObject);
    }

    public void RemoveItemObject(ItemSpawner itemObject)
    {
        itemIntersaction.Remove(itemObject);
    }

    public void ItemPickUp(InputAction.CallbackContext context)
    {
        if (!player.playerStateController.canPickup) return;
        if (itemIntersaction.Count == 0) return;

        ItemObject item = itemIntersaction[0].item;
        if (item.itemInfo.itemType == EItemType.Weapon)
        {
            PickedWeapon();
        }
        else
        {
            PickedNotWeapon();
        }
    }

    private void PickedWeapon()
    {
        ItemSpawner oldSpawner = itemIntersaction[0];
        ItemObject item = itemIntersaction[0].item;
        Transform parent = player.playerWeaponController.rightHand;
      

        String prevWeapon = player.playerWeaponController.currentWeaponType.ToString();
        int? magazine = player.playerWeaponController.currentMagazine;
        if (player.playerWeaponController.currentWeaponType == EWeaponType.Consumable)
        {
            prevWeapon = player.playerWeaponController.tempWeaponType.ToString();
        }

        var op = InstantiateAsync(WeaponSpawner);
        op.completed += (handle) =>
        {
            if (!handle.isDone) return;
            var labels = new List<string> { prevWeapon, "WeaponItems", "Items" };
            ItemSpawner spawner = op.Result[0].GetComponent<ItemSpawner>();
            spawner.transform.position = player.transform.position;
            if (magazine != null)
            {
                spawner.SetItem(labels, magazine.Value);
            }
            else
            {
                spawner.SetItem(labels, 0);
            }
        };


        Addressables.InstantiateAsync(item.itemInfo.itemAssetRef, parent).Completed += (handle) =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }
            WeaponBase weaponBase = handle.Result.GetComponent<WeaponBase>();
            player.playerWeaponController.ChangeWeapon(weaponBase);
            player.playerWeaponController.currentMagazine = item.count;
        };

        itemIntersaction.RemoveAt(0);
        Addressables.ReleaseInstance(item.gameObject);
        Destroy(oldSpawner.gameObject);
    }

    private void PickedNotWeapon()
    {
        ItemSpawner spawner = itemIntersaction[0];
        ItemObject item = itemIntersaction[0].item;

        if (player.playerSlotController.AddItem(item.itemInfo, item.count))
        {
            itemIntersaction.RemoveAt(0);
            Addressables.ReleaseInstance(item.gameObject);
            Destroy(spawner.gameObject);
            return;
        }
        //Debug
        Debug.Log("아이템을 Slot에 넣지 못했습니다.");
    }

    private void OnNumberKey(InputAction.CallbackContext context)
    {
        if (!player.playerStateController.canChangeWeapon)
        {
            return;
        }
        PlayerSlotController slotController = player.playerSlotController;
        PlayerWeaponController weaponController = player.playerWeaponController; 
        String keyNumber = context.control.displayName;

        if (int.TryParse(keyNumber, out int result))
        {
            ItemInfo itemInfp = slotController.slotItem(result - 1);
            if (itemInfp == null)
            {
                return;
            }
            if (prevItemInfo == itemInfp)
            {
                prevItemInfo = null;
                OffNumberKey();
                return;
            }

            prevItemInfo = itemInfp;
            weaponController.OnConsumable(itemInfp.itemAssetRef);
        }
    }

    public void OffNumberKey()
    {
        PlayerWeaponController weaponController = player.playerWeaponController;
        weaponController.OffConsumable();
    }
}
