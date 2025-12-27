using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

public abstract class ConsumableBase : WeaponBase
{
    [field: SerializeField] protected ItemInfo itemInfo { get; private set; }
    protected float currentFireDelay { get; set; }
    protected Transform owner { get; private set; }
    private void Awake()
    {
        currentFireDelay = Time.time - fireDelay;
    }

    public override void Equip(Transform owner)
    {
        this.owner = owner;
    }

    public override void Unequip(UnityAction onUnequip)
    {
        Addressables.Release(gameObject);
    }

    protected float CalcTheta(float v, float g, float y, float x)
    {
        float v2 = v * v;
        float v4 = v2 * v2;

        float discriminant = v4 - g * (2 * y * v2 + g * x * x);
        if (discriminant < 0)
        {
            Debug.Log("±ËÀû ¹üÀ§¿¡ ³Ñ¾î °©´Ï´Ù.");
            return 0;
        }

        float tan = (v2 - Mathf.Sqrt(discriminant)) / (g * x);
        return Mathf.Atan(tan);
    }
}
