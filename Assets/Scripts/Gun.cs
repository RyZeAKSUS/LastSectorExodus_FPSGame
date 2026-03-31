using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour
{
    [Header("Configuração")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.1f;
    public int magazineSize = 30;
    public float reloadTime = 2f;

    [Header("Munição")]
    public int reserveAmmo = 90;
    public int maxReserveAmmo = 120;

    [Header("Referências")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public GameObject bulletPrefab;
    public TextMeshProUGUI ammoText;

    private int _bulletsLeft;
    private bool _isReloading;
    private float _nextFireTime;

    void Start()
    {
        _bulletsLeft = magazineSize;
        UpdateAmmoUI();
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

        if (Input.GetButton("Fire1") && Time.time >= _nextFireTime && _bulletsLeft > 0)
        {
            _nextFireTime = Time.time + fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magazineSize && reserveAmmo > 0)
        {
            StartCoroutine(Reload());
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

        yield return new WaitForSeconds(reloadTime);

        int bulletsNeeded = magazineSize - _bulletsLeft;
        int bulletsToLoad = Mathf.Min(bulletsNeeded, reserveAmmo);

        _bulletsLeft += bulletsToLoad;
        reserveAmmo -= bulletsToLoad;

        _isReloading = false;
        UpdateAmmoUI();
    }

    public void AddAmmo(int amount)
    {
        reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
        UpdateAmmoUI();
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = _isReloading ? "Recarregando..." : _bulletsLeft + " | " + reserveAmmo;
    }
}