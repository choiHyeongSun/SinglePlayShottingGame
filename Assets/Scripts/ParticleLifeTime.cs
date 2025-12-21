using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleLifeTime : MonoBehaviour
{
    private ParticleSystem particleSystem;
    private ObjectPoolingManager poolManager;

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        poolManager = FindFirstObjectByType<ObjectPoolingManager>();
    }

    private void OnEnable()
    {
        StartCoroutine(ParticleLife());
    }

    private IEnumerator ParticleLife()
    {
        while (!particleSystem.isStopped)
        {
            yield return null;
        }

        PoolObject poolObject = GetComponent<PoolObject>();
        if (poolObject != null)
        {
            poolManager.PushObject(poolObject.originReference, gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
