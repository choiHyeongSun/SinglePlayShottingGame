using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperBullet : BulletBase
{
    private static readonly int MAX_HIT_COUNT = 10;
    private Vector3 prevPosition;
    private RaycastHit[] hits = new RaycastHit[MAX_HIT_COUNT];
    private HashSet<int> isAlreadyContactObject = new();

    protected virtual void Update()
    {
        float distance = Vector3.Distance(originPosition, transform.position);
        if (distance > maxRange && !isRelease)
        {
            isRelease = true;
            StartCoroutine(ReleaseBullet());
        }
    }

    private void FixedUpdate()
    {
        var capsule = bulletCollider as CapsuleCollider;
        if (capsule == null || isRelease)
        {
            return;
        }

        float radius = capsule.radius * transform.localScale.x;
        float dist = Vector3.Distance(prevPosition, transform.position);
        int count = Physics.SphereCastNonAlloc(prevPosition, radius, transform.forward, hits, dist);

        for (int i = 0; i < count; i++)
        {
            if (transform != hits[i].transform)
            {
                ContactObject(hits[i]);
            }
        }
        prevPosition = transform.position;
    }

    public override void Shoot(Vector3 direction, float force, float range)
    {
        base.Shoot(direction, force, range);
        prevPosition = transform.position;
        isAlreadyContactObject.Clear();

    }

    private void ContactObject(RaycastHit other)
    {
        int hashCode = other.transform.GetHashCode();
        if (isAlreadyContactObject.Contains(hashCode))
        {
            return;
        }
        int environmentLayer = LayerMask.NameToLayer("Environment");
        int layer = other.transform.gameObject.layer;
        isAlreadyContactObject.Add(hashCode);


        if (environmentLayer == layer)
        {
            isRelease = true;
            
            GameObject hitEffect = objectPoolingManager.PopObject(hitRef);
            hitEffect.transform.position = other.point;
            hitEffect.transform.rotation = Quaternion.LookRotation(other.normal);
            StartCoroutine(ReleaseBullet());

            rigid.velocity = Vector3.zero;
            bulletCollider.enabled = false;
            mesh.enabled = false;
        }
    }

    private IEnumerator ReleaseBullet()
    {
        isRelease = true;
        var poolObject = GetComponent<PoolObject>();
        yield return new WaitForSeconds(releaseDelay);
        objectPoolingManager.PushObject(poolObject.originReference, gameObject);
    }
}
