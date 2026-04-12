using UnityEngine;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public const int WeaponCount = 5;
    public const int CosmeticCount = 4;
    public const int MaxCosmeticStack = 3;

    [Header("Referências")]
    public GunSwitcher gunSwitcher;
    public InventoryUI inventoryUI;
    public Transform handItemHolder;

    [Header("Definições de itens")]
    public ItemDefinition[] weaponDefinitions = new ItemDefinition[5];
    public ItemDefinition[] cosmeticDefinitions = new ItemDefinition[4];

    [Header("Armas iniciais (índices)")]
    public int[] startingWeaponIndices = { 0, 1 };

    private bool[] _weaponOwned = new bool[WeaponCount];
    private int[] _cosmeticCounts = new int[CosmeticCount];
    private int _activeSlot = 0;
    private bool _isOpen = false;
    private GameObject _handCosmetic;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        foreach (int idx in startingWeaponIndices)
        {
            if (idx >= 0 && idx < WeaponCount)
            {
                _weaponOwned[idx] = true;
            }
        }

        _activeSlot = startingWeaponIndices.Length > 0 ? startingWeaponIndices[0] : 0;
        RefreshEquipped();
        inventoryUI?.Refresh(this);
    }

    void Update()
    {
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _isOpen = !_isOpen;
            ApplyInventoryState(_isOpen);
        }

        if (_isOpen)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _isOpen = false;
                ApplyInventoryState(false);
            }
            return;
        }

        if (PauseMenu.gameIsPaused) return;

        HandleHotkeys();
        HandleScroll();
        HandleCosmeticUse();
    }

    void HandleHotkeys()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { TryEquipSlot(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { TryEquipSlot(1); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { TryEquipSlot(2); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { TryEquipSlot(3); }
        if (Input.GetKeyDown(KeyCode.Alpha5)) { TryEquipSlot(4); }
        if (Input.GetKeyDown(KeyCode.Alpha6)) { TryEquipSlot(5); }
        if (Input.GetKeyDown(KeyCode.Alpha7)) { TryEquipSlot(6); }
        if (Input.GetKeyDown(KeyCode.Alpha8)) { TryEquipSlot(7); }
        if (Input.GetKeyDown(KeyCode.Alpha9)) { TryEquipSlot(8); }
    }

    void HandleScroll()
    {
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0f) return;

        int direction = scroll > 0f ? -1 : 1;
        int totalSlots = WeaponCount + CosmeticCount;
        int next = _activeSlot;

        for (int i = 0; i < totalSlots; i++)
        {
            next = (next + direction + totalSlots) % totalSlots;
            if (SlotHasItem(next))
            {
                TryEquipSlot(next);
                return;
            }
        }
    }

    bool SlotHasItem(int slot)
    {
        if (slot < 5)
        {
            return _weaponOwned[slot];
        }
        else
        {
            return _cosmeticCounts[slot - 5] > 0;
        }
    }

    public void TryEquipSlot(int slot)
    {
        if (slot < 5)
        {
            if (!_weaponOwned[slot]) return;
        }
        else
        {
            int cosIdx = slot - 5;
            if (_cosmeticCounts[cosIdx] <= 0) return;
        }

        _activeSlot = slot;
        RefreshEquipped();
        inventoryUI?.Refresh(this);
    }

    void HandleCosmeticUse()
    {
        if (_activeSlot < 5) return;

        int cosIdx = _activeSlot - 5;
        if (_cosmeticCounts[cosIdx] <= 0) return;
        if (!Input.GetMouseButtonDown(0)) return;

        ItemDefinition def = cosmeticDefinitions[cosIdx];
        if (def != null)
        {
            if (cosIdx == 0 || cosIdx == 1)
            {
                FindFirstObjectByType<PlayerHealth>()?.Heal(def.healAmount);
            }
            else if (cosIdx == 2)
            {
                HungerThirst.Instance?.AddHunger(def.hungerAmount);
            }
            else if (cosIdx == 3)
            {
                HungerThirst.Instance?.AddThirst(def.thirstAmount);
            }
        }

        _cosmeticCounts[cosIdx]--;

        if (_cosmeticCounts[cosIdx] <= 0)
        {
            SwitchToFirstAvailableWeapon();
        }
        else
        {
            HUDItemInfo.Instance?.UpdateCosmeticCount(_cosmeticCounts[cosIdx], MaxCosmeticStack);
            inventoryUI?.Refresh(this);
        }
    }

    void SwitchToFirstAvailableWeapon()
    {
        for (int i = 0; i < WeaponCount; i++)
        {
            if (_weaponOwned[i])
            {
                _activeSlot = i;
                RefreshEquipped();
                inventoryUI?.Refresh(this);
                return;
            }
        }

        _activeSlot = 0;
        gunSwitcher?.HideAllWeapons();
        gunSwitcher?.ClearAllGunUI();
        DestroyHandCosmetic();
        HUDItemInfo.Instance?.Hide();
        inventoryUI?.Refresh(this);
    }

    void ApplyInventoryState(bool open)
    {
        if (open)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        inventoryUI?.Refresh(this);
    }

    void RefreshEquipped()
    {
        gunSwitcher?.HideAllWeapons();
        gunSwitcher?.ClearAllGunUI();
        DestroyHandCosmetic();

        if (_activeSlot < 5)
        {
            if (_weaponOwned[_activeSlot])
            {
                if (_activeSlot == 0)
                {
                    HUDItemInfo.Instance?.Hide();
                }
                else
                {
                    HUDItemInfo.Instance?.ShowGun();
                }
                gunSwitcher?.EquipWeaponPublic(_activeSlot);
            }
        }
        else
        {
            int cosIdx = _activeSlot - 5;
            if (_cosmeticCounts[cosIdx] > 0)
            {
                HUDItemInfo.Instance?.ShowCosmetic(_cosmeticCounts[cosIdx], MaxCosmeticStack);

                ItemDefinition def = cosmeticDefinitions[cosIdx];
                if (def != null && def.worldPrefab != null && handItemHolder != null)
                {
                    _handCosmetic = Instantiate(def.worldPrefab, handItemHolder);
                    _handCosmetic.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
                    _handCosmetic.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                    _handCosmetic.transform.localScale = Vector3.one * 0.3f;
                }
            }
        }
    }

    void DestroyHandCosmetic()
    {
        if (_handCosmetic != null)
        {
            Destroy(_handCosmetic);
            _handCosmetic = null;
        }
    }

    public bool CanPickupWeapon(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= WeaponCount) return false;
        return !_weaponOwned[weaponIndex];
    }

    public bool CanPickupCosmetic(int cosmeticIndex)
    {
        if (cosmeticIndex < 0 || cosmeticIndex >= CosmeticCount) return false;
        return _cosmeticCounts[cosmeticIndex] < MaxCosmeticStack;
    }

    public bool TryPickupWeapon(int weaponIndex)
    {
        if (!CanPickupWeapon(weaponIndex)) return false;

        _weaponOwned[weaponIndex] = true;
        _activeSlot = weaponIndex;
        RefreshEquipped();
        inventoryUI?.Refresh(this);
        return true;
    }

    public bool TryPickupCosmetic(int cosmeticIndex)
    {
        if (!CanPickupCosmetic(cosmeticIndex)) return false;

        _cosmeticCounts[cosmeticIndex]++;

        if (_activeSlot == cosmeticIndex + 5)
        {
            HUDItemInfo.Instance?.UpdateCosmeticCount(_cosmeticCounts[cosmeticIndex], MaxCosmeticStack);
        }

        inventoryUI?.Refresh(this);
        return true;
    }

    public bool HasWeapon(int index)
    {
        return index >= 0 && index < WeaponCount && _weaponOwned[index];
    }

    public int GetCosmeticCount(int index)
    {
        if (index < 0 || index >= CosmeticCount) return 0;
        return _cosmeticCounts[index];
    }

    public bool GetIsOpen() => _isOpen;
    public int GetActiveSlot() => _activeSlot;
}