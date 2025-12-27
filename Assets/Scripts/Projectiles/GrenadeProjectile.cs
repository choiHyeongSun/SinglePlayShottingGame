using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : BulletBase
{
    private static readonly int MAX_HIT_COUNT = 1;

    RaycastHit[] hits = new RaycastHit[MAX_HIT_COUNT];
    private Vector3 prevPosition;

    public override void Shoot(Vector3 direction, float force, float range)
    {
        rigid.velocity = direction * force;
        prevPosition = transform.position;
    }

    private void FixedUpdate()
    {
        var sphere = bulletCollider as SphereCollider;
        if (sphere == null || isRelease)
        {
            return;
        }

        float radius = sphere.radius * transform.localScale.x;
        float dist = Vector3.Distance(prevPosition, transform.position);
        Vector3 dir = transform.position - prevPosition;
        int count = Physics.SphereCastNonAlloc(prevPosition, radius, dir.normalized, hits, dist);

        if (count != 0)
        {
            if (transform != hits[0].transform)
            {
                ContactObject(hits[0]);
            }
        }
        prevPosition = transform.position;
    }

    private void ContactObject(RaycastHit other)
    {
        if (other.transform == owner)
        {
            return;
        }
        int environmentLayer = LayerMask.NameToLayer("Environment");
        int layer = other.transform.gameObject.layer;

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

            CameraShake cameraShake = FindFirstObjectByType<CameraShake>();
            cameraShake.StartCameraShake(1.5f, 1.0f, 0.3f);
        }
    }

    private IEnumerator ReleaseBullet()
    {
        var poolObject = GetComponent<PoolObject>();
        yield return new WaitForSeconds(releaseDelay);
        objectPoolingManager.PushObject(poolObject.originReference, gameObject);
    }
}
