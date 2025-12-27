using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingState : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player player = animator.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        player.playerRollingController.OffRolling();
    }
}
