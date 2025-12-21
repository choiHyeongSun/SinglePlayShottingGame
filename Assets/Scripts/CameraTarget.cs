using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    private Player player;
    [field: SerializeField] private float maxRange;
    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void LateUpdate()
    {
        transform.position = player.transform.position + player.transform.forward * maxRange;
    }
}
