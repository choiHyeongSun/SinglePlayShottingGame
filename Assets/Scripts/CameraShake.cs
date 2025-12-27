using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private CinemachineVirtualCamera virutalCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    private Coroutine cameraShake;

    private void Awake()
    {
        virutalCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virutalCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        noise.m_AmplitudeGain = 0.0f;
        noise.m_FrequencyGain = 0.0f;
    }

    public void StartCameraShake(float amplitude, float frequency, float time)
    {
        if (cameraShake != null)
        {
            StopCoroutine(cameraShake);
        }
        cameraShake = StartCoroutine(CameraShakeCoroutine(amplitude, frequency, time));
    }

    private IEnumerator CameraShakeCoroutine(float amplitude, float frequency, float time)
    {
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;

        yield return new WaitForSeconds(time);
        noise.m_AmplitudeGain = 0.0f;
        noise.m_FrequencyGain = 0.0f;

    }
}
