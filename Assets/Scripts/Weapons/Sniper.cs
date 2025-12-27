using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Sniper : GunBase
{
    [SerializeField] private AssetReference shotParticle;

    private ObjectPoolingManager poolManager;
    private float currentFireTime;
    private Animator animator;
    private PlayerStateController playerStateController;

    protected override void Awake()
    {
        base.Awake();
        poolManager = FindFirstObjectByType<ObjectPoolingManager>();
        poolManager.RegisterPooling(bulletRef);
        poolManager.RegisterPooling(shotParticle);

        currentFireTime = Time.time - currentFireTime;
    }

    public override void Fire()
    {
        if (currentFireTime > Time.time || isReloading)
        {
            return;
        }

        if (!playerStateController.canFire)
        {
            return;
        }

        if (currentMagazine == 0)
        {
            if (!isReloading)
            {
                Reload();
            }
            return;
        }

        animator.SetTrigger("Fire_Trigger");
        CreateParticle();
        BulletBase bullet = CreateBullet();
        Vector3 euler = ProcessAccuracy();
        Quaternion rot = Quaternion.Euler(euler);
        Vector3 dir = rot * Vector3.forward;
        currentFireTime = Time.time + fireDelay;

        bullet.Shoot(dir, bulletForce, range);
        currentMagazine--;
    }
    public override void FireCanceled()
    {
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

    private BulletBase CreateBullet()
    {
        GameObject obj = poolManager.PopObject(bulletRef);
        BulletBase bullet = obj.GetComponent<BulletBase>();
        bullet.transform.position = muzzle.position;
        return bullet;
    }

    private void CreateParticle()
    {
        GameObject particle = poolManager.PopObject(shotParticle);
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
}
