using UnityEngine;

public class WeaponWallCheck : MonoBehaviour
{
    [Header("Configuração")]
    public float defaultWallCheckDistance = 0.8f;
    public float transitionSpeed = 8f;
    public LayerMask wallMask;

    [Header("Referências")]
    public Transform weaponHolder;
    public GunSwitcher gunSwitcher;

    [Header("Posição de repouso (perto da parede)")]
    public Vector3 loweredPositionOffset = new Vector3(0.1f, -0.3f, 0.1f);
    public Vector3 loweredRotationOffset = new Vector3(40f, -30f, -20f);

    public static bool IsWeaponLowered { get; private set; }

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;

    void Start()
    {
        _originalPosition = weaponHolder.localPosition;
        _originalRotation = weaponHolder.localRotation;
    }

    void Update()
    {
        float checkDistance = GetCurrentWeaponDistance();

        Ray ray = new Ray(transform.position, transform.forward);
        bool wallNear = Physics.Raycast(ray, checkDistance, wallMask);

        IsWeaponLowered = wallNear;

        Vector3 targetPosition = wallNear
            ? _originalPosition + loweredPositionOffset
            : _originalPosition;

        Quaternion targetRotation = wallNear
            ? _originalRotation * Quaternion.Euler(loweredRotationOffset)
            : _originalRotation;

        weaponHolder.localPosition = Vector3.Lerp(
            weaponHolder.localPosition,
            targetPosition,
            Time.deltaTime * transitionSpeed
        );

        weaponHolder.localRotation = Quaternion.Lerp(
            weaponHolder.localRotation,
            targetRotation,
            Time.deltaTime * transitionSpeed
        );
    }

    float GetCurrentWeaponDistance()
    {
        if (gunSwitcher == null) return defaultWallCheckDistance;

        int active = InventorySystem.Instance.GetActiveSlot();

        if (active == 0)
        {
            Knife knife = weaponHolder.GetComponentInChildren<Knife>(true);
            if (knife != null)
            {
                return knife.wallCheckDistance;
            }
        }
        else if (active >= 1 && active <= 4)
        {
            Gun gun = gunSwitcher.weapons[active].GetComponent<Gun>();
            if (gun != null)
            {
                return gun.wallCheckDistance;
            }
        }

        return defaultWallCheckDistance;
    }
}