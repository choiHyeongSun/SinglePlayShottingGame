using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private Transform rigTarget;


    private bool isFiring;
    private Player player;
    private IWeapon rifle;

    private Rig gunRig;

    public IWeapon currentWeapon { get; private set; }
   
    private void Awake()
    {
        player = GetComponent<Player>();
        rifle = GetComponentInChildren<IWeapon>();
        gunRig = GetComponentInChildren<Rig>();
    }

    private void Start()
    {
        PlayerInput input = player.playerInputController.playerInput;
        input.Player.OnFire.performed += OnFire;
        input.Player.OnFire.canceled += OnFireCanceled;
        input.Player.OnReload.performed += OnReload;

        ChangeWeapon(rifle);
    }

    private void OnFireCanceled(InputAction.CallbackContext obj)
    {
        if (currentWeapon == null) return;
        currentWeapon.FireCanceled();
    }

    private void OnFire(InputAction.CallbackContext obj)
    {
        if (currentWeapon == null) return;
        currentWeapon.Fire();
    }
    private void OnReload(InputAction.CallbackContext obj)
    {
        GunBase gun = currentWeapon as GunBase;

        if (gun == null)
        {
            return;
        }
        gun.Reload();
    }

    private void ChangeWeapon(IWeapon newWeapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.Unequip();
            GunBase gun = currentWeapon as GunBase;
            if (gun != null)
            {
                gun.OnRigPosition = null;
                gun.OnRigEulerAngle = null;
            }

        }
        if (newWeapon != null)
        {
            GunBase gun = newWeapon as GunBase;
            if (gun != null)
            {
                gun.OnRigPosition = (weight) => gunRig.weight = weight;
                gun.OnRigEulerAngle = (pos, euler) =>
                {
                    rigTarget.localPosition = pos;
                    rigTarget.localEulerAngles = euler;
                };

            }
            newWeapon.Equip(transform);

        }
        currentWeapon = newWeapon;
    }


}
