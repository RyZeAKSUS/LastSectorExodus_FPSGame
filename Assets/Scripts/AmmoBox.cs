using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [Header("Configuração")]
    public int ammoAmount = 30;

    [Header("Referências")]
    public GameObject lid;
    public GameObject indicatorPrefab;

    private bool _isAvailable = true;
    private GameObject _indicator;

    void Start()
    {
        if (indicatorPrefab != null)
        {
            _indicator = Instantiate(
                indicatorPrefab,
                transform.position + Vector3.up * 1.5f,
                Quaternion.identity
            );
        }
    }

    void Update()
    {
        if (_indicator != null)
        {
            _indicator.transform.position = transform.position + Vector3.up * 1.5f;
        }
    }

    public bool IsAvailable() => _isAvailable;

    public bool IsAmmoFull()
    {
        if (InventorySystem.Instance == null) return true;

        int activeSlot = InventorySystem.Instance.GetActiveSlot();
        if (activeSlot <= 0 || activeSlot > 4) return true;

        GunSwitcher gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        if (gunSwitcher == null) return true;

        Gun activeGun = gunSwitcher.weapons[activeSlot].GetComponent<Gun>();
        if (activeGun == null) return true;

        int totalCapacity = activeGun.magazineSize + activeGun.maxReserveAmmo;
        int currentTotal = activeGun.GetBulletsLeft() + activeGun.reserveAmmo;
        return currentTotal >= totalCapacity;
    }

    public void Collect()
    {
        if (!_isAvailable) return;

        if (InventorySystem.Instance == null) return;

        int activeSlot = InventorySystem.Instance.GetActiveSlot();
        if (activeSlot <= 0 || activeSlot > 4) return;

        GunSwitcher gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        if (gunSwitcher == null) return;

        Gun activeGun = gunSwitcher.weapons[activeSlot].GetComponent<Gun>();
        if (activeGun == null) return;

        activeGun.AddAmmo(ammoAmount);
        activeGun.ForceUpdateUI();

        _isAvailable = false;

        if (lid != null)
        {
            lid.SetActive(false);
        }

        if (_indicator != null)
        {
            Destroy(_indicator);
        }
    }
}