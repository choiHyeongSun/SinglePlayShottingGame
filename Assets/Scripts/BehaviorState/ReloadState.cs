using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player player = animator.gameObject.GetComponent<Player>();
        GunBase gunBase = player.playerWeaponController.currentWeaponBase as GunBase;
        if (gunBase == null)
        {
            return;
        }
        gunBase.animationReloading = false;
    }

}
