using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    [SerializeField] private float offsetY;
    private Player player;
    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }
    private void LateUpdate()
    {
        transform.position = player.playerTargetingController.pickingPosition + Vector3.up * offsetY;
    }
}
