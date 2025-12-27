using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaPotion : ConsumableBase
{
    [SerializeField] private float manaWeight;
    public override void Fire()
    {
        if (currentFireDelay > Time.time)
        {
            return;
        }
        PlayerSlotController playerSlotController = owner.GetComponent<PlayerSlotController>();
        playerSlotController.UseItem(itemInfo);
        Debug.Log("Mana È¸º¹");
        currentFireDelay = Time.time;

        if (!playerSlotController.ExistItem(itemInfo))
        {
            PlayerItemController playerItemController = owner.GetComponent<PlayerItemController>();
            playerItemController.OffNumberKey();
        }
    }

    public override void FireCanceled()
    {

    }
}
