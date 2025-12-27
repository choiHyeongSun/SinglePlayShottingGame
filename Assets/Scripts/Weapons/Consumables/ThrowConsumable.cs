using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ThrowConsumable : ConsumableBase
{
    [SerializeField] private float range;
    [SerializeField] private AssetReference assetRef;
    [SerializeField] private float throwDelay;
    private ObjectPoolingManager poolingManager;
    private float force;
    private bool isFire;
    private Vector3 direction;

    private PlayerTrajectoryController playerTrajectory;
    private PlayerSlotController playerSlotController;
    private PlayerItemController playerItemController;
    private PlayerTargetingController playerTargetingController;
    private PlayerStateController playerStateController;

    private Animator animator;

    private void Awake()
    {
        poolingManager = FindFirstObjectByType<ObjectPoolingManager>();
        poolingManager.RegisterPooling(assetRef);
        currentFireDelay = Time.time - fireDelay;
    }

    public override void Equip(Transform owner)
    {
        base.Equip(owner);

        playerTrajectory = owner.GetComponent<PlayerTrajectoryController>();
        playerSlotController = owner.GetComponent<PlayerSlotController>();
        playerItemController = owner.GetComponent<PlayerItemController>();
        playerTargetingController = owner.GetComponent<PlayerTargetingController>();
        playerStateController = owner.GetComponent<PlayerStateController>();

        animator = owner.GetComponent<Animator>();
        animator.SetInteger("WeaponType", (int)weaponType);
    }

    public override void Fire()
    {
        if (currentFireDelay >= Time.time)
        {
            return;
        }
        owner.GetComponent<PlayerTrajectoryController>().EnableLineRenderer();

        isFire = true;
        force = range * 1.1f;
    }

    public override void FireCanceled()
    {
        if (!isFire)
        {
            return;
        }

        StartCoroutine(ThrowDelay());
    }


    public override void Unequip(UnityAction onUnequip)
    {
        base.Unequip(onUnequip);
        playerTrajectory.DisableLineRenderer();
    }

    private void FixedUpdate()
    {
        if (!isFire)
        {
            return;
        }

        float g = -Physics.gravity.y;
        int count = playerTrajectory.count;
        Vector3 mousePos = playerTargetingController.pickingPosition;
        Vector3 playerPos = transform.position;

        Vector3 dir = mousePos - playerPos;
        float dist = new Vector3(dir.x, 0.0f, dir.z).magnitude;
        if (range - dir.y <= dist)
        {
            dist = range - dir.y;
        }
        float theta = CalcTheta(force, -Physics.gravity.y, dir.y, dist);
        float tanTheta = Mathf.Tan(theta);
        float cosTheta = Mathf.Cos(theta);

        Vector3 planeDirection = new Vector3(dir.x, 0.0f, dir.z).normalized;
        Vector3 launchVelocity = (planeDirection * Mathf.Cos(theta) + Vector3.up * Mathf.Sin(theta));
        direction = launchVelocity;
        Vector3 currentPos = transform.position;

        for (int i = 0; i < count; i++)
        {
            float currentX = (dist / (count - 1)) * i;
            float currentY = (currentX * tanTheta) - (g * currentX * currentX) / (2 * force * force * cosTheta * cosTheta);
            Vector3 pointPos = currentPos + (planeDirection * currentX) + (Vector3.up * currentY);
            playerTrajectory.SetPosition(i, pointPos);
        }
        playerTrajectory.ApplyPostions();
    }

    private IEnumerator ThrowDelay()
    {
        isFire = false;
        playerTrajectory.DisableLineRenderer();
        playerSlotController.UseItem(itemInfo);
        animator.SetTrigger("Throw");
        currentFireDelay = Time.time + fireDelay;

        yield return new WaitForSeconds(throwDelay);

        BulletBase projectile = poolingManager.PopObject(assetRef).GetComponent<BulletBase>();
        projectile.owner = owner;
        projectile.transform.position = transform.position;
        projectile.Shoot(direction, force, 0);

        if (!playerSlotController.ExistItem(itemInfo))
        {
            playerItemController.OffNumberKey();
        }

        playerStateController.BeginThrow();
    }
}
