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
        
        StartCoroutine(ReleaseBullet());
    }
    protected virtual void Update()
    {
        float distance = Vector3.Distance(originPosition, transform.position);
        if (distance > maxRange && !isRelease)
        {
            StartCoroutine(ReleaseBullet());
        }
    }
    private IEnumerator ReleaseBullet()
    {
        var poolObject = GetComponent<PoolObject>();
        yield return new WaitForSeconds(releaseDelay);
        objectPoolingManager.PushObject(poolObject.originReference, gameObject);
    }
}
