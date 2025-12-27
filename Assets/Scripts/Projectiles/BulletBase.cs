using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BulletBase : MonoBehaviour
{
    protected bool isRelease = false;

    protected TrailRenderer trail;
    protected Rigidbody rigid;
    protected Collider bulletCollider;
    protected ObjectPoolingManager objectPoolingManager;
    protected float maxRange;
    protected Vector3 originPosition;
    protected float releaseDelay = 2.0f;

    [SerializeField] protected AssetReference hitRef;

    protected MeshRenderer mesh;
    public Transform owner { get; set; }

    protected virtual void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        objectPoolingManager = FindFirstObjectByType<ObjectPoolingManager>();

        mesh = GetComponent<MeshRenderer>();
        bulletCollider = GetComponent<Collider>();
        objectPoolingManager.RegisterPooling(hitRef);
    }

    public virtual void Shoot(Vector3 direction, float force, float range)
    {
        originPosition = transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
        rigid.AddForce(direction * force, ForceMode.Impulse);
        maxRange = range;
    }
    protected virtual void OnDisable()
    {
        trail.Clear();
        rigid.velocity = Vector3.zero;
        mesh.enabled = true;
        bulletCollider.enabled = true;
        isRelease = false;
    }
}
