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

    [Header("Referências")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public GameObject bulletPrefab;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI fireModeText;

    [Header("Muzzle Flash")]
    public ParticleSystem muzzleFlash;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Recuo")]
    public float recoilAmount = 0.05f;
    public float recoilRotation = 2f;
    public float recoilSpeed = 10f;
    public float recoilReturnSpeed = 6f;

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

        _originalLocalPosition = transform.localPosition;
        _originalLocalRotation = transform.localRotation;
        _targetLocalPosition = _originalLocalPosition;
        _targetLocalRotation = _originalLocalRotation;

        if (reloadBarObject != null)
        {
            reloadBarObject.SetActive(false);
        }
        if (reloadBar != null)
        {
            reloadBar.value = 0f;
        }

        UpdateAmmoUI();
        UpdateFireModeUI();
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

        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        ApplyRecoil();

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range)
            ? hit.point
            : ray.GetPoint(range);

        Vector3 direction = (targetPoint - muzzlePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Bullet>().damage = damage;

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
        {
            audioSource.PlayOneShot(reloadSound);
        }

        if (reloadBarObject != null)
        {
            reloadBarObject.SetActive(true);
        }

        float elapsed = 0f;
        while (elapsed < reloadTime)
        {
            elapsed += Time.deltaTime;
            if (reloadBar != null)
            {
                reloadBar.value = elapsed / reloadTime;
            }
            yield return null;
        }

        if (reloadBarObject != null)
        {
            reloadBarObject.SetActive(false);
        }

        int bulletsNeeded = magazineSize - _bulletsLeft;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);
        _bulletsLeft += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        _isReloading = false;
        UpdateAmmoUI();
    }

    void OnDisable()
    {
        if (_isReloading)
        {
            StopAllCoroutines();
            _isReloading = false;
            if (reloadBarObject != null)
            {
                reloadBarObject.SetActive(false);
            }
            if (reloadBar != null)
            {
                reloadBar.value = 0f;
            }
            UpdateAmmoUI();
        }

        transform.localPosition = _originalLocalPosition;
        transform.localRotation = _originalLocalRotation;
        _targetLocalPosition = _originalLocalPosition;
        _targetLocalRotation = _originalLocalRotation;
    }

    public void AddAmmo(int amount)
    {
        int totalCapacity = magazineSize + maxReserveAmmo;
        int currentTotal = _bulletsLeft + reserveAmmo;
        int canAdd = totalCapacity - currentTotal;
        reserveAmmo += Mathf.Min(amount, canAdd);
        UpdateAmmoUI();
    }

    public void ForceUpdateUI()
    {
        UpdateAmmoUI();
        UpdateFireModeUI();
    }

    public int GetBulletsLeft() => _bulletsLeft;

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
            {
                fireModeText.text = "<color=#FFD700>Automático</color>  |  Manual";
            }
            else
            {
                fireModeText.text = "Automático  |  <color=#FFD700>Manual</color>";
            }
        }
    }
}