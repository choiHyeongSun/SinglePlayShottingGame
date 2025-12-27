using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRollingController : MonoBehaviour
{
    [SerializeField] private float layerWeightSmooth;
    private Player player;
    private float rigWeightTempSave;
    

    public bool isRolling { get; private set; }

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        PlayerInput playerInput = player.playerInputController.playerInput;
        playerInput.Player.OnRolling.performed += OnRolling;
    }

    private void OnRolling(InputAction.CallbackContext context)
    {
        if (player.playerWeaponController.currentWeaponType == EWeaponType.Consumable)
        {
            return;
        }
        if (!player.playerStateController.canRolling)
        {
            return;
        }

        if (player.playerMoveController.moveDirection.magnitude < 1E-4)
        {
            return;
        }

        int fulllayerIndex = player.animator.GetLayerIndex("FullBody");
        player.playerStateController.BeginRolling();
        player.animator.SetLayerWeight(fulllayerIndex, 1.0f);
        player.animator.SetTrigger("Rolling");
        player.playerWeaponController.rigWeight = 0.0f;
        isRolling = true;
    }

    public void OffRolling()
    {
        //rig은 애니메이션 과정에서 호출시 적용되지 않기 때문에
        //코루틴을 활용하여 다음 업데이트에 호출되도록 설정
        StartCoroutine(NextFrameOffRolling());
    }


    private IEnumerator NextFrameOffRolling()
    {
        int layerIndex = player.animator.GetLayerIndex("FullBody");
        float layerWeigth = player.animator.GetLayerWeight(layerIndex);
        while (layerWeigth > 0.0f)
        {
            layerWeigth -= Time.deltaTime * layerWeightSmooth;
            player.animator.SetLayerWeight(layerIndex, layerWeigth);
            yield return null;
        }
        player.animator.SetLayerWeight(layerIndex, 0.0f);
        player.playerStateController.EndRolling();
        player.playerWeaponController.rigWeight = 1.0f;
        isRolling = false;
    }
}
