using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class GunBase : MonoBehaviour ,IWeapon
{
    private readonly float rigSmoothTime = 5.0f;

    [Header("GunBase")]
    [SerializeField] protected EWeaponType weaponType;
    [SerializeField] protected float fireDelay;
    [SerializeField] protected float range;
    [SerializeField] protected AssetReference bulletRef;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected float bulletForce;
    [SerializeField] protected int maxMagazine;
     
    [SerializeField] protected float accuracy;

    [SerializeField] protected Vector3 rigPosition;
    [SerializeField] protected Vector3 rigEulerAngle;

    public UnityAction<float> OnRigPosition;
    public UnityAction<Vector3, Vector3> OnRigEulerAngle;

    protected int currentMagazine { get; set; }

    protected Transform owner { get; set; }
    public bool isReloading { get; protected set; }

    public bool animationReloading { get; set; }

    protected virtual void Awake()
    {
        currentMagazine = maxMagazine;
    }

    public abstract void Fire();
    public abstract void FireCanceled();
    public abstract void Equip(Transform owner);
    public abstract void Unequip();

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
            OnRigPosition(weight);
            yield return null;
        }
        while (weight <= 1.0f - 1E-4)
        {
            weight = Mathf.Clamp(weight + Time.deltaTime * rigSmoothTime, 0.0f, 1.0f);
            OnRigPosition(weight);
            yield return null;
        }

        isReloading = false;
        currentMagazine = maxMagazine;
        OnRigPosition(1.0f);
    }
}
