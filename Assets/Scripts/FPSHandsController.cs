using UnityEngine;

public class FPSHandsController : MonoBehaviour
{
    [Header("Referências")]
    public Transform leftShoulder;
    public SkinnedMeshRenderer handsMesh;
    public GunSwitcher gunSwitcher;
    public Transform weaponHolder;

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

        FollowWeaponAnimation();
    }

    void UpdateHands()
    {
        if (InventorySystem.Instance == null) return;

        int slot = InventorySystem.Instance.GetActiveSlot();

        if (slot >= 5)
        {
            if (handsMesh != null)
            {
                handsMesh.enabled = false;
            }
            _activeWeaponTransform = null;
            return;
        }

        if (handsMesh != null)
        {
            handsMesh.enabled = true;
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

        Vector3 positionDelta = _activeWeaponTransform.localPosition - _weaponBasePosition;
        Quaternion rotationDelta = _activeWeaponTransform.localRotation
            * Quaternion.Inverse(_weaponBaseRotation);

        transform.localPosition = _handsBasePosition
            + positionDelta * animationFollowStrength;
        transform.localRotation = Quaternion.Slerp(
            _handsBaseRotation,
            rotationDelta * _handsBaseRotation,
            animationFollowStrength
        );
    }
}