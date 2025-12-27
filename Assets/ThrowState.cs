using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowState : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player player = animator.GetComponent<Player>();
        player.playerStateController.EndThrow();
    }
}
