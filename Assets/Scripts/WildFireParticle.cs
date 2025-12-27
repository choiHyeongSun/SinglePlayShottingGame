using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WildFireParticle : MonoBehaviour
{
    [SerializeField] private float lifeTime;
    private ObjectPoolingManager poolManager;
    private ParticleSystem[] particleSystems;

    private void Awake()
    {
        poolManager = FindFirstObjectByType<ObjectPoolingManager>();
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void OnEnable()
    {
        StartCoroutine(WideFireParticleLifeTime());
    }



    private IEnumerator WideFireParticleLifeTime()
    {
        PoolObject poolObject = GetComponent<PoolObject>();
        yield return new WaitForSeconds(lifeTime);

        for (int i = 0; i < particleSystems.Length; i++)
        {
            particleSystems[i].Stop();
        }

        for (int i = 0; i < particleSystems.Length; i++)
        {
            while (!particleSystems[i].isStopped)
            {
                yield return null;
            }
        }
        poolManager.PushObject(poolObject.originReference, poolObject.gameObject);
    }
}
