using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class SniperBullet : BulletBase
{
    private static readonly int MAX_HIT_COUNT = 10;
    private Vector3 prevPosition;
    RaycastHit[] hits = new RaycastHit[MAX_HIT_COUNT];

    private HashSet<int> isAlreadyContactObject = new();
    protected virtual void Update()
    {
        float distance = Vector3.Distance(originPosition, transform.position);
        if (distance > maxRange && !isRelease)
        {
            isRelease = true;
            StartCoroutine(ReleaseBullet(null));
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

            Ray ray = new Ray();
            ray.origin = originPosition;
            ray.direction = transform.forward;

            GameObject hitEffect = objectPoolingManager.PopObject(hitRef);
            hitEffect.transform.position = other.point;
            hitEffect.transform.rotation = Quaternion.LookRotation(other.normal);
            StartCoroutine(ReleaseBullet(hitEffect));

            rigid.velocity = Vector3.zero;
            bulletCollider.enabled = false;
            mesh.enabled = false;
        }
    }

    private IEnumerator ReleaseBullet(GameObject particle)
    {
        isRelease = true;
        var poolObject = GetComponent<PoolObject>();
        if (particle == null)
        {
            objectPoolingManager.PushObject(poolObject.originReference, gameObject);
            yield break;
        }

        ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
        ;
        while (!particleSystem.isStopped)
        {
            yield return null;
        }
        objectPoolingManager.PushObject(hitRef, particle);
        objectPoolingManager.PushObject(poolObject.originReference, gameObject);
    }
}
