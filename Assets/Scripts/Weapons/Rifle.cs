using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

public class Rifle : GunBase
{
    [SerializeField] private AssetReference muzzleParticle;

    protected bool isFiring;
    private ObjectPoolingManager poolManager;
    private Coroutine fireCoroutine;
    Animator animator;
    
    protected override void Awake()
    {
        base.Awake();
        poolManager = FindFirstObjectByType<ObjectPoolingManager>();
        poolManager.RegisterPooling(bulletRef);
        poolManager.RegisterPooling(muzzleParticle);
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
        if (animator != null)
        {
            int upperBody = animator.GetLayerIndex("UpperBody");
            animator.SetLayerWeight(upperBody, 1.0f);
        }
        OnRigPosition(1.0f);
        OnRigEulerAngle(rigPosition, rigEulerAngle);
        animator.SetInteger("WeaponType", (int)weaponType);
    }

    public override void Unequip()
    {
        if (animator != null)
        {
            int upperBody = animator.GetLayerIndex("UpperBody");
            animator.SetLayerWeight(upperBody, 0.0f);
        }
        
        StopAllCoroutines();
        FireCanceled();
        OnRigPosition(0.0f);

        isReloading = false;
        owner = null;
        animator = null;

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
