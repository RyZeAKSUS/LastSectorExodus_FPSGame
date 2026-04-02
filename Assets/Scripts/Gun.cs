using UnityEngine;
using TMPro;

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

    private int _bulletsLeft;
    private bool _isReloading;
    private float _nextFireTime;

    [Header("UI Recarga")]
    public UnityEngine.UI.Slider reloadBar;
    public GameObject reloadBarObject;

    void Start()
    {
        _bulletsLeft = magazineSize;
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
    }

    void HandleInput()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
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

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range) ? hit.point : ray.GetPoint(range);

        Vector3 direction = (targetPoint - muzzlePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Bullet>().damage = damage;
    }

    System.Collections.IEnumerator Reload()
    {
        _isReloading = true;
        UpdateAmmoUI();

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
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
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
}