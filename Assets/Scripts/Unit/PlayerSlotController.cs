using System;
using UnityEngine;

public class PlayerSlotController : MonoBehaviour
{
    private static readonly int slotMaxCount = 9;
    private ItemSlot[] itemSlots = new ItemSlot[slotMaxCount];
    public ItemInfo slotItem(int index) => itemSlots[index].itemInfo;
    public int slotItemCount(int index) => itemSlots[index].count;
    

    public void Awake()
    {
        for (int i = 0; i < slotMaxCount; i++)
        {
            itemSlots[i] = new ItemSlot();
        }
    }

    public bool AddItem(ItemInfo itemInfo, int count)
    {
        if (itemInfo.itemType == EItemType.Weapon)
        {
            return false;
        }

        for (int i = 0; i < slotMaxCount; i++)
        {
            if (itemSlots[i].itemInfo == itemInfo)
            {
                itemSlots[i].count += count;
                return true;
            }
        }

        for (int i = 0; i < slotMaxCount; i++)
        {
            if (itemSlots[i].itemInfo == null)
            {
                itemSlots[i].itemInfo = itemInfo;
                itemSlots[i].count += count;
                return true;
            }
        }
        return false;
    }

    public bool UseItem(int slotIndex)
    {
        if (itemSlots[slotIndex].itemInfo == null)
        {
            return false;
        }

        itemSlots[slotIndex].count--;
        if (itemSlots[slotIndex].count == 0)
        {
            itemSlots[slotIndex].itemInfo = null;
        }
        return true;
    }

    public bool UseItem(ItemInfo itemInfo)
    {
        int notFound = -1;
        int itemIndex = notFound;
        for (int i = 0; i < slotMaxCount; i++)
        {
            if (itemSlots[i].itemInfo == itemInfo)
            {
                itemIndex = i;
                break;
            }
        }

        if (itemIndex != notFound)
        {
            return UseItem(itemIndex);
        }
        return false;
    }

    public bool ExistItem(ItemInfo itemInfo)
    {
        for (int i = 0; i < slotMaxCount; i++)
        {
            if (itemSlots[i].itemInfo == itemInfo)
            {
                return true;
            }
        }
        return false;
    }

}
