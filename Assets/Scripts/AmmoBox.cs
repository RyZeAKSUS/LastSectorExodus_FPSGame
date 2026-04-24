using UnityEngine;
using System.Collections;

public class AmmoBox : MonoBehaviour
{
    [Header("Configuração")]
    public float respawnTime = 15f;

    [Header("Referências")]
    public GameObject lid;
    public GameObject indicatorPrefab;

    private bool _isAvailable = true;
    private bool _isCollecting = false;
    private GameObject _indicator;

    void Start()
    {
        CreateIndicator();
    }

    void Update()
    {
        if (_indicator != null)
        {
            _indicator.transform.position = transform.position + Vector3.up * 1.5f;
        }
    }

    public bool IsAvailable()
    {
        return _isAvailable;
    }

    public bool IsAmmoFull()
    {
        Gun activeGun = GetActiveGun();
        if (activeGun == null) return true;

        int totalCapacity = activeGun.magazineSize + activeGun.maxReserveAmmo;
        int currentTotal = activeGun.GetBulletsLeft() + activeGun.reserveAmmo;

        return currentTotal >= totalCapacity;
    }

    public void Collect()
    {
        if (!_isAvailable || _isCollecting) return;

        Gun activeGun = GetActiveGun();
        if (activeGun == null) return;

        if (IsAmmoFull()) return;

        _isCollecting = true;

        int amountToGive = activeGun.magazineSize + activeGun.maxReserveAmmo;
        int amountAdded = activeGun.AddAmmo(amountToGive);

        if (amountAdded <= 0)
        {
            _isCollecting = false;
            return;
        }

        activeGun.ForceUpdateUI();

        _isAvailable = false;

        if (lid != null)
            lid.SetActive(false);

        if (_indicator != null)
            Destroy(_indicator);

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        _isAvailable = true;
        _isCollecting = false;

        if (lid != null)
            lid.SetActive(true);

        CreateIndicator();
    }

    void CreateIndicator()
    {
        if (indicatorPrefab != null && _indicator == null)
        {
            _indicator = Instantiate(
                indicatorPrefab,
                transform.position + Vector3.up * 1.5f,
                Quaternion.identity
            );
        }
    }

    Gun GetActiveGun()
    {
        if (InventorySystem.Instance == null) return null;

        int activeSlot = InventorySystem.Instance.GetActiveSlot();

        if (activeSlot <= 0 || activeSlot > 4) return null;

        GunSwitcher gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        if (gunSwitcher == null) return null;

        if (gunSwitcher.weapons == null) return null;
        if (activeSlot >= gunSwitcher.weapons.Length) return null;
        if (gunSwitcher.weapons[activeSlot] == null) return null;

        return gunSwitcher.weapons[activeSlot].GetComponent<Gun>();
    }
}