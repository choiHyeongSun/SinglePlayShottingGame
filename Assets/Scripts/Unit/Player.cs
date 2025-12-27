using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour, IDamaage
{
    public PlayerItemController playerItemController { get; private set; }
    public PlayerInputController playerInputController { get; private set; }
    public PlayerTargetingController playerTargetingController { get; private set; }
    public PlayerWeaponController playerWeaponController { get; private set; }
    public PlayerStateController playerStateController { get; private set; }
    public PlayerMoveController playerMoveController { get; private set; }
    public PlayerRollingController playerRollingController { get; private set; }
    public PlayerSlotController playerSlotController { get; private set; }
    public PlayerTrajectoryController playerTrajectoryController { get; private set; }
    public Animator animator { get; private set; }
  
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerTargetingController = GetComponent<PlayerTargetingController>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        playerItemController = GetComponent<PlayerItemController>();
        playerStateController = GetComponent<PlayerStateController>();
        playerMoveController = GetComponent<PlayerMoveController>();
        playerRollingController = GetComponent<PlayerRollingController>();
        playerSlotController = GetComponent<PlayerSlotController>();
        playerTrajectoryController = GetComponent<PlayerTrajectoryController>();
        animator = GetComponent<Animator>();

    }

    public void SendDamage(float InDamage)
    {

    }
}
