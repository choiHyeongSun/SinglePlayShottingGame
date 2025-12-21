using System.Collections;
using UnityEngine;


public class NormalBullet : BulletBase
{
    private void OnCollisionEnter(Collision other)
    {
        rigid.velocity = Vector3.zero;
        bulletCollider.enabled = false;
        mesh.enabled = false;
        isRelease = true;

        ContactPoint[] contact = new ContactPoint[other.contactCount];
        other.GetContacts(contact);
        GameObject hitEffect = objectPoolingManager.PopObject(hitRef);
        hitEffect.transform.position = contact[0].point;
        hitEffect.transform.rotation = Quaternion.LookRotation(contact[0].normal);
        StartCoroutine(ReleaseBullet(hitEffect));
    }
    protected virtual void Update()
    {
        float distance = Vector3.Distance(originPosition, transform.position);
        if (distance > maxRange && !isRelease)
        {
            StartCoroutine(ReleaseBullet(null));
        }
    }
    private IEnumerator ReleaseBullet(GameObject particle)
    {
        var poolObject = GetComponent<PoolObject>();
        if (particle == null)
        {
            objectPoolingManager.PushObject(poolObject.originReference, gameObject);
            yield break;
        }

        ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
        while (!particleSystem.isStopped)
        {
            yield return null;
        }

        objectPoolingManager.PushObject(poolObject.originReference, gameObject);
        objectPoolingManager.PushObject(hitRef, particle);
    }
}
