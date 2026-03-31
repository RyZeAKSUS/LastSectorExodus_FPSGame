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
        UpdateAmmoUI();
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

        if (Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        _bulletsLeft--;

        // Spawna a bala no muzzlePoint apontada para o centro do ecrã
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, range) 
                              ? hit.point 
                              : ray.GetPoint(range);

        Vector3 direction = (targetPoint - muzzlePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Bullet>().damage = damage;
    }

    System.Collections.IEnumerator Reload()
    {
        _isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        _bulletsLeft = magazineSize;
        _isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = _isReloading ? "Recarregando..." : _bulletsLeft + "/" + magazineSize;
        }
    }

    public void AddAmmo(int amount)
    {
        _bulletsLeft = Mathf.Min(_bulletsLeft + amount, magazineSize);
        UpdateAmmoUI();
    }
}