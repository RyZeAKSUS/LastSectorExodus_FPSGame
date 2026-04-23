using UnityEngine;

public class FPSHandsController : MonoBehaviour
{
    [Header("Referências")]
    public Transform leftShoulder;
    public SkinnedMeshRenderer handsMesh;
    public GunSwitcher gunSwitcher;
    public Transform weaponHolder;
    public Transform animationOffset;

    [Header("Posições por arma")]
    public Vector3[] handPositions = new Vector3[5];
    public Vector3[] handRotations = new Vector3[5];

    [Header("Multiplicador de animação")]
    [Range(0f, 1f)]
    public float animationFollowStrength = 0.6f;

    private int _lastSlot = -1;
    private Transform _activeWeaponTransform;
    private Vector3 _weaponBasePosition;
    private Quaternion _weaponBaseRotation;
    private Vector3 _handsBasePosition;
    private Quaternion _handsBaseRotation;

    void Start()
    {
        UpdateHands();
    }

    void Update()
    {
        if (InventorySystem.Instance == null) return;

        int slot = InventorySystem.Instance.GetActiveSlot();

        if (slot != _lastSlot)
        {
            _lastSlot = slot;
            UpdateHands();
        }

        if (handsMesh != null)
        {
            bool sniperScoped = slot == 4
                && ADSSystem.Instance != null
                && ADSSystem.Instance.IsAiming;

            bool cosmeticActive = slot >= 5;

            handsMesh.enabled = !sniperScoped && !cosmeticActive;
        }

        FollowWeaponAnimation();
    }

    void UpdateHands()
    {
        if (InventorySystem.Instance == null) return;

        int slot = InventorySystem.Instance.GetActiveSlot();

        if (slot >= 5)
        {
            _activeWeaponTransform = null;
            ResetAnimationOffset();
            return;
        }

        bool twoHanded = slot == 2 || slot == 3 || slot == 4;

        if (leftShoulder != null)
        {
            leftShoulder.localScale = twoHanded ? Vector3.one : Vector3.zero;
        }

        _handsBasePosition = handPositions[slot];
        _handsBaseRotation = Quaternion.Euler(handRotations[slot]);
        transform.localPosition = _handsBasePosition;
        transform.localRotation = _handsBaseRotation;

        ResetAnimationOffset();

        if (slot == 0 && weaponHolder != null)
        {
            Knife knife = weaponHolder.GetComponentInChildren<Knife>(true);
            _activeWeaponTransform = knife != null ? knife.transform : null;
        }
        else if (gunSwitcher != null && slot >= 1 && slot <= 4)
        {
            Gun gun = gunSwitcher.weapons[slot].GetComponent<Gun>();
            _activeWeaponTransform = gun != null ? gun.transform : null;
        }
        else
        {
            _activeWeaponTransform = null;
        }

        if (_activeWeaponTransform != null)
        {
            _weaponBasePosition = _activeWeaponTransform.localPosition;
            _weaponBaseRotation = _activeWeaponTransform.localRotation;
        }
    }

    void FollowWeaponAnimation()
    {
        if (_activeWeaponTransform == null) return;
        if (animationOffset == null) return;

        Vector3 positionDelta = _activeWeaponTransform.localPosition - _weaponBasePosition;
        Quaternion rotationDelta = _activeWeaponTransform.localRotation
            * Quaternion.Inverse(_weaponBaseRotation);

        animationOffset.localPosition = positionDelta * animationFollowStrength;
        animationOffset.localRotation = Quaternion.Slerp(
            Quaternion.identity,
            rotationDelta,
            animationFollowStrength
        );
    }

    void ResetAnimationOffset()
    {
        if (animationOffset == null) return;
        animationOffset.localPosition = Vector3.zero;
        animationOffset.localRotation = Quaternion.identity;
    }
}