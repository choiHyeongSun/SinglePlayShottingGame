using UnityEngine;
using UnityEngine.Events;


public enum EWeaponType
{
    Rifle,
    Sniper, 
    Shotgun,
    Pistol,
    Consumable
}
public abstract class WeaponBase: MonoBehaviour
{
    [field: SerializeField, Header("WeaponBase")]
    public EWeaponType weaponType { get; private set; }

    [SerializeField] protected float fireDelay;
    public abstract void Fire();
    public abstract void FireCanceled();
    public abstract void Equip(Transform owner);
    public abstract void Unequip(UnityAction onUnequip = null);
}
