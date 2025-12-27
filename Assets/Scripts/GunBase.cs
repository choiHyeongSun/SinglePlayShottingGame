using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class GunBase : WeaponBase
{
    private readonly float rigSmoothTime = 5.0f;


    [Header("GunBase")]
    [SerializeField] protected float range;
    [SerializeField] protected AssetReference bulletRef;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected float bulletForce;
    [SerializeField] protected int maxMagazine;

    [SerializeField] protected float accuracy;

    [SerializeField] protected Vector3 rigPosition;
    [SerializeField] protected Vector3 rigEulerAngle;

    public UnityAction<float> OnRigWeightAndPos;
    public UnityAction<Vector3, Vector3> OnRigEulerAngle;
    
    protected Transform owner;
    public int currentMagazine { get; protected set; }
    public bool isReloading { get; protected set; }
    public bool animationReloading { get; set; }



    public void SetMagazine(int magazineCount) => currentMagazine = magazineCount;
    protected virtual void Awake() => currentMagazine = maxMagazine;

    protected virtual void OnDisable()
    {
        isReloading = false;
        animationReloading = false;
    }

    public override void Unequip(UnityAction onUnequip)
    {
        onUnequip?.Invoke();
        OnRigWeightAndPos(0.0f);
        Addressables.ReleaseInstance(gameObject);
    }

    public void Reload()
    {
        if (!isReloading && currentMagazine < maxMagazine)
        {
            StartCoroutine(StartReload());
        }
    }

    protected IEnumerator StartReload()
    {
        var animator = owner.GetComponent<Animator>();
        animator.SetTrigger("Reload");
        isReloading = true;
        animationReloading = true;
        float weight = 1.0f;

        while (animationReloading)
        {
            weight = Mathf.Clamp(weight - Time.deltaTime * rigSmoothTime, 0.0f, 1.0f);
            OnRigWeightAndPos(weight);
            yield return null;
        }
        while (weight <= 1.0f - 1E-4)
        {
            weight = Mathf.Clamp(weight + Time.deltaTime * rigSmoothTime, 0.0f, 1.0f);
            OnRigWeightAndPos(weight);
            yield return null;
        }

        OnRigWeightAndPos(1.0f);
        isReloading = false;
        currentMagazine = maxMagazine;
    }
}
