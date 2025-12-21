using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EWeaponType
{
    Rifle,
    Sniper, 
    Shotgun,
    Ninja,
}
public interface IWeapon
{
    public void Fire();
    public void FireCanceled();
    public void Equip(Transform owner);
    public void Unequip();
}
