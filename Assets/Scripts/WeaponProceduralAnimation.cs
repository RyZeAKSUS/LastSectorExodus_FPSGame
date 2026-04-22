using UnityEngine;

public class WeaponProceduralAnimation : MonoBehaviour
{
    [Header("Referências")]
    public CharacterController characterController;
    public GunSwitcher gunSwitcher;
    public Transform weaponHolder;

    [Header("Idle - respiração")]
    public float idleBobSpeed = 1.2f;
    public float idleBobAmountY = 0.01f;
    public float idleRotationAmount = 0.3f;

    [Header("Walk - balanço")]
    public float walkBobSpeed = 8f;
    public float walkBobAmountY = 0.01f;
    public float walkBobAmountX = 0.003f;

    [Header("Run - balanço pronunciado")]
    public float runBobSpeed = 12f;
    public float runBobAmountY = 0.014f;
    public float runBobAmountX = 0.006f;
    public float runTiltAmount = 2f;

    [Header("Reload - arma desce")]
    public float reloadLowerAmount = 0.1f;
    public float reloadLowerSpeed = 6f;

    [Header("Weapon Sway - movimento da câmara")]
    public float swayAmount = 0.03f;
    public float swayRotationAmount = 1.7f;
    public float swaySmoothing = 6f;
    public float swayMaxAmount = 0.06f;

    [Header("Suavização")]
    public float smoothSpeed = 8f;

    private Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private float _bobTimer = 0f;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;
    private Vector3 _swayPosition;
    private Quaternion _swayRotation;

    void Start()
    {
        _originalPosition = weaponHolder.localPosition;
        _originalRotation = weaponHolder.localRotation;
        _targetPosition = _originalPosition;
        _targetRotation = _originalRotation;
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;

        if (InventorySystem.Instance == null) return;

        int slot = InventorySystem.Instance.GetActiveSlot();

        float speed = characterController != null
            ? new Vector3(
                characterController.velocity.x,
                0f,
                characterController.velocity.z).magnitude
            : 0f;

        bool isRunning = speed > 4f;
        bool isWalking = speed > 0.1f && !isRunning;

        bool isReloading = slot >= 1 && slot <= 4 && IsCurrentGunReloading(slot);

        CalculateSway();
        CalculateTarget(isWalking, isRunning, isReloading, speed);

        Vector3 finalPosition = _targetPosition + _swayPosition;
        Quaternion finalRotation = _targetRotation * _swayRotation;

        weaponHolder.localPosition = Vector3.Lerp(
            weaponHolder.localPosition,
            finalPosition,
            Time.deltaTime * smoothSpeed
        );
        weaponHolder.localRotation = Quaternion.Slerp(
            weaponHolder.localRotation,
            finalRotation,
            Time.deltaTime * smoothSpeed
        );
    }

    void CalculateSway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        float swayX = Mathf.Clamp(-mouseX * swayAmount, -swayMaxAmount, swayMaxAmount);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -swayMaxAmount, swayMaxAmount);

        Vector3 targetSwayPosition = new Vector3(swayX, swayY, 0f);

        float rotX = Mathf.Clamp(mouseY * swayRotationAmount, -swayRotationAmount * 2f, swayRotationAmount * 2f);
        float rotY = Mathf.Clamp(mouseX * swayRotationAmount, -swayRotationAmount * 2f, swayRotationAmount * 2f);
        float rotZ = Mathf.Clamp(mouseX * swayRotationAmount, -swayRotationAmount * 2f, swayRotationAmount * 2f);

        Quaternion targetSwayRotation = Quaternion.Euler(rotX, rotY, -rotZ);

        _swayPosition = Vector3.Lerp(
            _swayPosition,
            targetSwayPosition,
            Time.deltaTime * swaySmoothing
        );

        _swayRotation = Quaternion.Slerp(
            _swayRotation,
            targetSwayRotation,
            Time.deltaTime * swaySmoothing
        );
    }

    void CalculateTarget(bool isWalking, bool isRunning, bool isReloading, float speed)
    {
        _targetPosition = _originalPosition;
        _targetRotation = _originalRotation;

        if (isReloading)
        {
            _targetPosition += new Vector3(0f, -reloadLowerAmount, 0f);
            return;
        }

        if (isRunning)
        {
            _bobTimer += Time.deltaTime * runBobSpeed;
            float bobY = Mathf.Sin(_bobTimer) * runBobAmountY;
            float bobX = Mathf.Cos(_bobTimer * 0.5f) * runBobAmountX;
            _targetPosition += new Vector3(bobX, bobY, 0f);
            _targetRotation = _originalRotation * Quaternion.Euler(0f, 0f, -runTiltAmount);
        }
        else if (isWalking)
        {
            _bobTimer += Time.deltaTime * walkBobSpeed;
            float bobY = Mathf.Sin(_bobTimer) * walkBobAmountY;
            float bobX = Mathf.Cos(_bobTimer * 0.5f) * walkBobAmountX;
            _targetPosition += new Vector3(bobX, bobY, 0f);
        }
        else
        {
            _bobTimer += Time.deltaTime * idleBobSpeed;
            float breathY = Mathf.Sin(_bobTimer) * idleBobAmountY;
            float breathRot = Mathf.Sin(_bobTimer * 0.5f) * idleRotationAmount;
            _targetPosition += new Vector3(0f, breathY, 0f);
            _targetRotation = _originalRotation * Quaternion.Euler(breathRot, 0f, 0f);
        }
    }

    bool IsCurrentGunReloading(int slot)
    {
        if (gunSwitcher == null) return false;

        Gun gun = gunSwitcher.weapons[slot].GetComponent<Gun>();
        if (gun != null)
        {
            return gun.IsReloading();
        }
        return false;
    }

    public void ResetToOriginal()
    {
        _originalPosition = weaponHolder.localPosition;
        _originalRotation = weaponHolder.localRotation;
    }
}