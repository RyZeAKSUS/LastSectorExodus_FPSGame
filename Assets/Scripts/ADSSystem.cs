using UnityEngine;
using System.Collections;

public class ADSSystem : MonoBehaviour
{
    public static ADSSystem Instance { get; private set; }

    [Header("Referências")]
    public Camera playerCamera;
    public GunSwitcher gunSwitcher;
    public CrosshairDynamic crosshairDynamic;
    public GameObject sniperScopeUI;

    [Header("Configuração")]
    public float adsSpeed = 10f;
    public float normalFOV = 50f;
    public float aimingSpeedMultiplier = 0.5f;

    [Header("Sniper Shake")]
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.08f;

    [Header("Sniper Respiração")]
    public float breathSpeed = 0.8f;
    public float breathAmount = 0.0015f;

    public bool IsAiming { get; private set; }
    public bool IsSniperScoped => _isSniperActive && IsAiming;
    public Vector3 CurrentADSPosition { get; private set; }
    public Vector3 CurrentADSRotation { get; private set; }

    private float _targetFOV;
    private bool _isSniperActive = false;
    private float _breathTimer = 0f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _targetFOV = normalFOV;
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return;
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return;
        if (WeaponWallCheck.IsWeaponLowered) return;

        int slot = InventorySystem.Instance != null
            ? InventorySystem.Instance.GetActiveSlot()
            : 0;

        bool canAim = slot >= 1 && slot <= 4;
        _isSniperActive = slot == 4;

        if (canAim && Input.GetMouseButton(1))
        {
            IsAiming = true;
            UpdateADSSettings(slot);
        }
        else
        {
            IsAiming = false;
            _targetFOV = normalFOV;
        }

        if (sniperScopeUI != null)
        {
            sniperScopeUI.SetActive(IsSniperScoped);
        }

        if (gunSwitcher != null && gunSwitcher.weapons.Length > 4)
        {
            GameObject sniperModel = gunSwitcher.weapons[4];
            if (sniperModel != null)
            {
                foreach (Renderer r in sniperModel.GetComponentsInChildren<Renderer>())
                {
                    r.enabled = !IsSniperScoped;
                }
            }
        }

        if (crosshairDynamic != null)
        {
            crosshairDynamic.SetAiming(IsAiming);
        }

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                _targetFOV,
                Time.deltaTime * adsSpeed
            );

            if (IsSniperScoped)
            {
                _breathTimer += Time.deltaTime * breathSpeed;
                float breathX = Mathf.Sin(_breathTimer) * breathAmount;
                float breathY = Mathf.Cos(_breathTimer * 0.7f) * breathAmount * 0.5f;

                Vector3 basePos = playerCamera.transform.parent != null
                    ? Vector3.zero
                    : playerCamera.transform.localPosition;

                playerCamera.transform.localPosition = new Vector3(
                    breathX,
                    breathY,
                    playerCamera.transform.localPosition.z
                );
            }
            else
            {
                _breathTimer = 0f;
                playerCamera.transform.localPosition = Vector3.zero;
            }
        }
    }

    void UpdateADSSettings(int slot)
    {
        if (gunSwitcher == null) return;

        Gun gun = gunSwitcher.weapons[slot].GetComponent<Gun>();
        if (gun == null) return;

        _targetFOV = gun.adsFOV;
        CurrentADSPosition = gun.adsPosition;
        CurrentADSRotation = gun.adsRotation;
    }

    public float GetSpeedMultiplier()
    {
        return IsAiming ? aimingSpeedMultiplier : 1f;
    }

    public void TriggerSniperShake()
    {
        if (IsSniperScoped)
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;
        Vector3 originalPos = playerCamera.transform.localPosition;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            playerCamera.transform.localPosition = new Vector3(
                originalPos.x + x,
                originalPos.y + y,
                originalPos.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.localPosition = originalPos;
    }
}