using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum EItemType
{
    Weapon = 0,
    Consumable,
}
[CreateAssetMenu(fileName = "ItemInfo" , menuName = "Create/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [field: SerializeField] public EItemType itemType { get; private set; }
    [field: SerializeField] public Sprite itemImage { get; private set; }
    [field: SerializeField] public String itemName { get; private  set; }
    [field: SerializeField, TextArea] public String itemDescription { get; private set; }
    [field: SerializeField] public AssetReference itemAssetRef { get; private set; }
}
