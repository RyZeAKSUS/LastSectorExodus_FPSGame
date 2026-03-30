using UnityEngine;
using TMPro;
using Unity.VisualScripting;

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
    public TextMeshProUGUI ammoText;

    private int _bulletsLeft;
    private bool _isReloading;
    private float _nextFireTime;

    void Start()
    {
        _bulletsLeft = magazineSize;
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

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    System.Collections.IEnumerator Reload()
    {
        _isReloading = true;
        Debug.Log("A recarregar...");

        yield return new WaitForSeconds(reloadTime);

        _bulletsLeft = magazineSize;
        _isReloading = false;
    }

    void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = _isReloading ? "A recarregar..." : _bulletsLeft + " / " + magazineSize;
        }
    }
}