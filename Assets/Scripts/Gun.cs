using UnityEngine;
using TMPro;
using System.Collections;

public class Gun : MonoBehaviour
{
    public enum FireMode { Automatic, Manual }

    [Header("Configuração")]
    public float damage = 25f;
    public float range = 100f;
    public float autoFireRate = 0.1f;
    public float manualFireRate = 0.4f;
    public int magazineSize = 30;
    public float reloadTime = 2f;
    public FireMode fireMode = FireMode.Automatic;
    public bool canSwitchFireMode = false;

    [Header("Munição")]
    public int reserveAmmo = 90;
    public int maxReserveAmmo = 120;

    [Header("Dispersão")]
    public float hipSpread = 0.03f;
    public float adsSpread = 0.005f;

    [Header("ADS")]
    public Vector3 adsPosition;
    public Vector3 adsRotation;
    public float adsFOV = 45f;

    [Header("Referências")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public GameObject bulletPrefab;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI fireModeText;

    [Header("Muzzle Flash")]
    public GameObject[] muzzleFlashes;

    [Header("Buraco de Bala")]
    public GameObject bulletHolePrefab;
    public float bulletHoleSize = 0.15f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Recuo")]
    public float recoilAmount = 0.05f;
    public float recoilRotation = 2f;
    public float recoilSpeed = 10f;
    public float recoilReturnSpeed = 6f;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.8f;

    [Header("UI Recarga")]
    public UnityEngine.UI.Slider reloadBar;
    public GameObject reloadBarObject;

    private int _bulletsLeft;
    private bool _isReloading;
    private float _nextFireTime;
    private Vector3 _originalLocalPosition;
    private Quaternion _originalLocalRotation;
    private Vector3 _targetLocalPosition;
    private Quaternion _targetLocalRotation;

    void Start()
    {
        _bulletsLeft = magazineSize;

        if (reloadBarObject != null)
            reloadBarObject.SetActive(false);

        if (reloadBar != null)
            reloadBar.value = 0f;

        UpdateAmmoUI();
        UpdateFireModeUI();
    }

    void OnEnable()
    {
        _originalLocalPosition = transform.localPosition;
        _originalLocalRotation = transform.localRotation;
        _targetLocalPosition = _originalLocalPosition;
        _targetLocalRotation = _originalLocalRotation;
    }

    void Update()
    {
        HandleInput();
        HandleRecoilReturn();
    }

    void HandleInput()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return;
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return;
        if (WeaponWallCheck.IsWeaponLowered) return;
        if (_isReloading) return;

        if (fireMode == FireMode.Automatic)
        {
            if (Input.GetButton("Fire1") && Time.time >= _nextFireTime && _bulletsLeft > 0)
            {
                _nextFireTime = Time.time + autoFireRate;
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= _nextFireTime && _bulletsLeft > 0)
            {
                _nextFireTime = Time.time + manualFireRate;
                Shoot();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magazineSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.Q) && canSwitchFireMode)
        {
            fireMode = fireMode == FireMode.Automatic ? FireMode.Manual : FireMode.Automatic;
            UpdateFireModeUI();
        }
    }

    void Shoot()
    {
        _bulletsLeft--;
        UpdateAmmoUI();

        if (fireSound != null)
        {
            AudioSource persistentSource = GetComponentInParent<GunSwitcher>()?.GetComponent<AudioSource>();

            if (persistentSource != null)
                persistentSource.PlayOneShot(fireSound);
            else if (audioSource != null)
                audioSource.PlayOneShot(fireSound);
        }

        bool isSniperScoped = ADSSystem.Instance != null && ADSSystem.Instance.IsSniperScoped;

        if (!isSniperScoped && muzzleFlashes != null && muzzleFlashes.Length > 0)
        {
            int randomIndex = Random.Range(0, muzzleFlashes.Length);

            if (muzzleFlashes[randomIndex] != null)
            {
                GameObject flash = Instantiate(
                    muzzleFlashes[randomIndex],
                    muzzlePoint.position,
                    muzzlePoint.rotation
                );

                flash.transform.SetParent(muzzlePoint);
                Destroy(flash, 0.05f);
            }
        }

        ApplyRecoil();
        ADSSystem.Instance?.TriggerSniperShake();

        bool isAiming = ADSSystem.Instance != null && ADSSystem.Instance.IsAiming;
        bool isCrouching = FindFirstObjectByType<PlayerMovement>()?.IsCrouching() ?? false;

        float currentSpread = isAiming
            ? adsSpread
            : (isCrouching ? hipSpread * 0.5f : hipSpread);

        Vector3 forward = playerCamera.transform.forward;
        forward += playerCamera.transform.right * Random.Range(-currentSpread, currentSpread);
        forward += playerCamera.transform.up * Random.Range(-currentSpread, currentSpread);
        forward.Normalize();

        Ray ray = new Ray(playerCamera.transform.position, forward);

        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range)
            ? hit.point
            : ray.GetPoint(range);

        if (bulletHolePrefab != null && Physics.Raycast(ray, out RaycastHit holeHit, range))
        {
            if (holeHit.collider.GetComponent<EnemyHealth>() == null &&
                holeHit.collider.GetComponent<HeadshotZone>() == null)
            {
                Vector3 holePos = holeHit.point + holeHit.normal * 0.005f;
                Quaternion holeRot = Quaternion.LookRotation(-holeHit.normal);

                GameObject hole = Instantiate(bulletHolePrefab, holePos, holeRot);
                hole.transform.localScale = Vector3.one * bulletHoleSize;

                Destroy(hole, 30f);
            }
        }

        Vector3 direction = (targetPoint - muzzlePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            muzzlePoint.position,
            Quaternion.LookRotation(direction)
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
            bulletScript.damage = damage;

        if (_bulletsLeft <= 0 && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
        }
    }

    void ApplyRecoil()
    {
        _targetLocalPosition = _originalLocalPosition + new Vector3(0f, 0f, -recoilAmount);
        _targetLocalRotation = _originalLocalRotation * Quaternion.Euler(-recoilRotation, 0f, 0f);
    }

    void HandleRecoilReturn()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            _targetLocalPosition,
            Time.deltaTime * recoilSpeed
        );

        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            _targetLocalRotation,
            Time.deltaTime * recoilSpeed
        );

        _targetLocalPosition = Vector3.Lerp(
            _targetLocalPosition,
            _originalLocalPosition,
            Time.deltaTime * recoilReturnSpeed
        );

        _targetLocalRotation = Quaternion.Lerp(
            _targetLocalRotation,
            _originalLocalRotation,
            Time.deltaTime * recoilReturnSpeed
        );
    }

    IEnumerator Reload()
    {
        _isReloading = true;
        UpdateAmmoUI();

        if (audioSource != null && reloadSound != null)
            audioSource.PlayOneShot(reloadSound);

        if (reloadBarObject != null)
            reloadBarObject.SetActive(true);

        float elapsed = 0f;

        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;

            if (reloadBar != null)
                reloadBar.value = elapsed / reloadTime;

            yield return null;
        }

        if (reloadBarObject != null)
            reloadBarObject.SetActive(false);

        if (reloadBar != null)
            reloadBar.value = 0f;

        int bulletsNeeded = magazineSize - _bulletsLeft;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        _bulletsLeft += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        _isReloading = false;
        UpdateAmmoUI();
    }

    void OnDisable()
    {
        CancelReloadIfNeeded();

        if (_originalLocalPosition != Vector3.zero)
        {
            transform.localPosition = _originalLocalPosition;
            transform.localRotation = _originalLocalRotation;
            _targetLocalPosition = _originalLocalPosition;
            _targetLocalRotation = _originalLocalRotation;
        }
    }

    public int AddAmmo(int amount)
    {
        CancelReloadIfNeeded();

        int totalCapacity = magazineSize + maxReserveAmmo;
        int currentTotal = _bulletsLeft + reserveAmmo;
        int canAdd = Mathf.Max(0, totalCapacity - currentTotal);

        int amountToAdd = Mathf.Min(amount, canAdd);

        reserveAmmo += amountToAdd;

        UpdateAmmoUI();

        return amountToAdd;
    }

    void CancelReloadIfNeeded()
    {
        if (!_isReloading) return;

        StopAllCoroutines();
        _isReloading = false;

        if (reloadBarObject != null)
            reloadBarObject.SetActive(false);

        if (reloadBar != null)
            reloadBar.value = 0f;

        UpdateAmmoUI();
    }

    public void ForceUpdateUI()
    {
        UpdateAmmoUI();
        UpdateFireModeUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText == null) return;

        ammoText.text = _bulletsLeft + " | " + reserveAmmo;
    }

    void UpdateFireModeUI()
    {
        if (fireModeText == null) return;

        if (!canSwitchFireMode)
        {
            fireModeText.gameObject.SetActive(false);
        }
        else
        {
            fireModeText.gameObject.SetActive(true);

            if (fireMode == FireMode.Automatic)
                fireModeText.text = "<color=#FFD700>Automático</color>  |  Manual";
            else
                fireModeText.text = "Automático  |  <color=#FFD700>Manual</color>";
        }
    }

    public int GetBulletsLeft()
    {
        return _bulletsLeft;
    }

    public bool IsReloading()
    {
        return _isReloading;
    }
}