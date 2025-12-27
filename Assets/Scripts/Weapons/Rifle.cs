using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Rifle : GunBase
{
    [SerializeField] private AssetReference muzzleParticle;

    protected bool isFiring;
    private float currentFireDelay;
    private ObjectPoolingManager poolManager;
    private Coroutine fireCoroutine;
    private Animator animator;
    private PlayerStateController playerStateController;
    protected override void Awake()
    {
        base.Awake();
        poolManager = FindFirstObjectByType<ObjectPoolingManager>();
        poolManager.RegisterPooling(bulletRef);
        poolManager.RegisterPooling(muzzleParticle);
        currentFireDelay = Time.time - fireDelay;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        isFiring = false;
    }

    public override void Fire() 
    {
        isFiring = true;
        ProcessReload();
        fireCoroutine = StartCoroutine(FireDelay());
    }
    public override void FireCanceled()
    {
        isFiring = false;
        if (fireCoroutine != null)
        {
            StopCoroutine(fireCoroutine);
        }
        animator.SetBool("Fire_Bool", false);
    }

    public override void Equip(Transform owner)
    {
        this.owner = owner;
        animator = owner.GetComponent<Animator>();
        playerStateController = owner.GetComponent<PlayerStateController>();

        if (animator != null)
        {
            int upperBody = animator.GetLayerIndex("UpperBody_Gun");
            animator.SetLayerWeight(upperBody, 1.0f);
        }
        OnRigWeightAndPos(1.0f);
        OnRigEulerAngle(rigPosition, rigEulerAngle);
        animator.SetInteger("WeaponType", (int)weaponType);
    }

    private IEnumerator FireDelay()
    {
        while (isFiring)
        {
            animator.SetBool("Fire_Bool", false);
            if (currentMagazine <= 0 || isReloading)
            {
                yield return null;
                continue;
            }
            if (!playerStateController.canFire || currentFireDelay > Time.time)
            {
                yield return null;
                continue;
            }
            currentFireDelay = Time.time + fireDelay;
            animator.SetBool("Fire_Bool", true);

            CreateParticle();
            BulletBase bullet = CreateBullet();
            Vector3 euler = ProcessAccuracy();
            Quaternion rot = Quaternion.Euler(euler);
            Vector3 dir = rot * Vector3.forward;

            bullet.Shoot(dir, bulletForce, range);
            currentMagazine--;
            yield return new WaitForSeconds(fireDelay);
            ProcessReload();
        }
    }

    private BulletBase CreateBullet()
    {
        GameObject obj = poolManager.PopObject(bulletRef);
        BulletBase bullet = obj.GetComponent<BulletBase>();
        bullet.transform.position = muzzle.position;
        return bullet;
    }

    private void CreateParticle()
    {
        GameObject particle = poolManager.PopObject(muzzleParticle);
        particle.transform.position = muzzle.position;
        particle.transform.rotation = muzzle.rotation;
    }

    private Vector3 ProcessAccuracy()
    {
        float halfaccuracy = accuracy * 0.5f;
        Vector3 euler = muzzle.eulerAngles + new Vector3(
            Random.Range(-halfaccuracy, halfaccuracy),
            Random.Range(-halfaccuracy, halfaccuracy),
            0);
        return euler;
    }

    private void ProcessReload()
    {
        if (currentMagazine <= 0)
        {
            if (!isReloading)
            {
                Reload();
            }
        }
    }
}
