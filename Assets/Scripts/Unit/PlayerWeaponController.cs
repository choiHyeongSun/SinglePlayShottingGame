using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class PlayerWeaponController : MonoBehaviour
{
    [field: SerializeField] public Transform rightHand { get; private set; }
    [SerializeField] private Transform rigTarget;
    [SerializeField] private AssetReference defaultWeapon;

    private Animator animator;
    private Player player;
    private Rig gunRig;
    private WeaponBase TempWeaponBase;

    public float rigWeight { get => gunRig.weight; set => gunRig.weight = value; }
    public WeaponBase currentWeaponBase { get; private set; }
    public EWeaponType currentWeaponType => currentWeaponBase.weaponType;
    public EWeaponType tempWeaponType => TempWeaponBase.weaponType;


    public int? currentMagazine
    {
        get
        {
            GunBase gunBase = currentWeaponBase as GunBase;
            if (gunBase == null)
            {
                return null;
            }
            return gunBase.currentMagazine;
        }

        set
        {
            GunBase gunBase = currentWeaponBase as GunBase;
            if (gunBase == null)
            {
                return;
            }

            if (value == null)
            {
                return;
            }
            if (value.Value < 0)
            {
                return;
            }
            gunBase.SetMagazine(value.Value);
        }
    }

    private void Awake()
    {
        player = GetComponent<Player>();
        gunRig = GetComponentInChildren<Rig>();
        animator = GetComponent<Animator>();
        Addressables.InstantiateAsync(defaultWeapon, rightHand).Completed += (handle) =>
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                return;
            }

            WeaponBase weaponBase = handle.Result.GetComponent<WeaponBase>();
            ChangeWeapon(weaponBase);
        };
    }

    private void Start()
    {
        PlayerInput input = player.playerInputController.playerInput;
        input.Player.OnFire.performed += OnFire;
        input.Player.OnFire.canceled += OnFireCanceled;
        input.Player.OnReload.performed += OnReload;
    }

    private void OnFireCanceled(InputAction.CallbackContext obj)
    {
        if (currentWeaponBase == null) return;
        currentWeaponBase.FireCanceled();
    }

    private void OnFire(InputAction.CallbackContext obj)
    {
        if (currentWeaponBase == null) return;
        currentWeaponBase.Fire();
    }
    private void OnReload(InputAction.CallbackContext obj)
    {
        GunBase gun = currentWeaponBase as GunBase;
        if (gun == null)
        {
            return;
        }
        gun.Reload();
    }

    public void ChangeWeapon(WeaponBase newWeaponBase)
    {
        if (!player.playerStateController.canChangeWeapon)
        {
            return;
        }
        if (currentWeaponBase != null)
        {
            currentWeaponBase.Unequip(() =>
            {
                int upperBody = animator.GetLayerIndex("UpperBody_Gun");
                animator.SetLayerWeight(upperBody, 0.0f);
            });
        }

        if (newWeaponBase != null)
        {
            GunBase gun = newWeaponBase as GunBase;
            if (gun != null)
            {
                gun.OnRigWeightAndPos = (weight) => rigWeight = weight;
                gun.OnRigEulerAngle = (pos, euler) =>
                {
                    rigTarget.localPosition = pos;
                    rigTarget.localEulerAngles = euler;
                };
            }
            newWeaponBase.Equip(transform);
        }
        currentWeaponBase = newWeaponBase;
    }

    public void OnConsumable(AssetReference assetRef)
    {
        if (currentWeaponBase == null)
        {
            return;
        }
        OffConsumable();
        TempWeaponBase = currentWeaponBase;
        currentWeaponBase.gameObject.SetActive(false);
        var handle = Addressables.InstantiateAsync(assetRef,rightHand);
        handle.WaitForCompletion();

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            return;
        }
        currentWeaponBase = handle.Result.GetComponent<WeaponBase>();
        currentWeaponBase.Equip(transform);
        rigWeight = 0.0f;
    }

    public void OffConsumable()
    {
        if (currentWeaponBase == null || currentWeaponType != EWeaponType.Consumable)
        {
            return;
        }

        currentWeaponBase.Unequip();
        currentWeaponBase = TempWeaponBase;
        currentWeaponBase.gameObject.SetActive(true);
        currentWeaponBase.Equip(transform);
        TempWeaponBase = null;
        rigWeight = 1.0f;
    }
}
