using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemObject : MonoBehaviour
{
    private static readonly int notCountNum = -1;
    [field: SerializeField] public ItemInfo itemInfo { get; private set; }
    public int count { get; set; } = notCountNum;
}
