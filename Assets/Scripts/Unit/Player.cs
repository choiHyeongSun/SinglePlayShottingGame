using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour, IDamaage
{

    public PlayerInputController playerInputController { get; private set; }
    public PlayerTargetingController playerTargetingController { get; private set; }
    public PlayerWeaponController playerWeaponController { get; private set; }
    public Animator animator { get; private set; }
    private void Awake()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerTargetingController = GetComponent<PlayerTargetingController>();
        playerWeaponController = GetComponent<PlayerWeaponController>();
        animator = GetComponent<Animator>();
    }

    public void SendDamage(float InDamage)
    {

    }
}
