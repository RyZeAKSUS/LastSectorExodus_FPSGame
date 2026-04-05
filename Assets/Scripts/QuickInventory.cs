using UnityEngine;

public class QuickInventory : MonoBehaviour
{
    public static QuickInventory Instance { get; private set; }

    [Header("Itens iniciais")]
    public ItemDefinition startItem1;
    public ItemDefinition startItem2;

    [Header("Referências")]
    public Transform handItemHolder;
    public QuickInventoryUI inventoryUI;

    private ItemDefinition[] _slots = new ItemDefinition[4];
    private GameObject[] _spawnedConsumables = new GameObject[4];
    private int _activeSlot = 0;
    private bool _isOpen = false;
    private GunSwitcher _gunSwitcher;

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
        _gunSwitcher = FindFirstObjectByType<GunSwitcher>();

        if (startItem1 != null) _slots[0] = startItem1;
        if (startItem2 != null) _slots[1] = startItem2;

        RefreshEquipped();
        inventoryUI?.Refresh(_slots, _activeSlot, _isOpen);
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

        if (_isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            _isOpen = false;
            ApplyInventoryState(false);
            return;
        }

        if (_isOpen || PauseMenu.gameIsPaused) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SetActiveSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SetActiveSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SetActiveSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) SetActiveSlot(3);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) SetActiveSlot((_activeSlot - 1 + 4) % 4);
        else if (scroll < 0f) SetActiveSlot((_activeSlot + 1) % 4);

        HandleItemUse();
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
        inventoryUI?.Refresh(_slots, _activeSlot, _isOpen);
    }

    void HandleItemUse()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        ItemDefinition item = _slots[_activeSlot];
        if (item == null) return;
        if (item.category == ItemCategory.Weapon) return;

        PlayerHealth health = FindFirstObjectByType<PlayerHealth>();
        if (health != null) health.Heal(item.healAmount);

        if (_spawnedConsumables[_activeSlot] != null)
        {
            Destroy(_spawnedConsumables[_activeSlot]);
            _spawnedConsumables[_activeSlot] = null;
        }
        _slots[_activeSlot] = null;

        RefreshEquipped();
        inventoryUI?.Refresh(_slots, _activeSlot, _isOpen);
    }

    void SetActiveSlot(int index)
    {
        _activeSlot = index;
        RefreshEquipped();
        inventoryUI?.Refresh(_slots, _activeSlot, _isOpen);
    }

    void RefreshEquipped()
    {
        for (int i = 0; i < _spawnedConsumables.Length; i++)
        {  
            if (_spawnedConsumables[i] != null)
            {
                _spawnedConsumables[i].SetActive(false);
            }
        }

        _gunSwitcher?.HideAllWeapons();

        ItemDefinition item = _slots[_activeSlot];
        if (item == null) return;

        if (item.category == ItemCategory.Weapon)
        {
            if (_gunSwitcher != null && item.weaponIndex >= 0)
            {    
                _gunSwitcher.EquipWeaponPublic(item.weaponIndex);
            }
            return;
        }

        if (item.worldPrefab == null) return;

        if (_spawnedConsumables[_activeSlot] == null)
        {
            _spawnedConsumables[_activeSlot] = Instantiate(item.worldPrefab, handItemHolder);
            _spawnedConsumables[_activeSlot].transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
            _spawnedConsumables[_activeSlot].transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            _spawnedConsumables[_activeSlot].transform.localScale = Vector3.one * 0.3f;
        }
        _spawnedConsumables[_activeSlot].SetActive(true);
    }

    public PickupResult TryPickup(ItemDefinition newItem, Vector3 pickupPosition)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i] == null)
            {
                _slots[i] = newItem;
                if (i == _activeSlot) RefreshEquipped();
                inventoryUI?.Refresh(_slots, _activeSlot, _isOpen);
                return new PickupResult { success = true, displaced = null };
            }
        }

        ItemDefinition displaced = _slots[_activeSlot];

        if (_spawnedConsumables[_activeSlot] != null)
        {
            Destroy(_spawnedConsumables[_activeSlot]);
            _spawnedConsumables[_activeSlot] = null;
        }

        _slots[_activeSlot] = newItem;
        RefreshEquipped();
        inventoryUI?.Refresh(_slots, _activeSlot, _isOpen);

        return new PickupResult { success = true, displaced = displaced };
    }

    public bool InventoryFull()
    {
        foreach (var s in _slots)
        {
            if (s == null) return false;
        }
        return true;
    }

    public ItemDefinition GetActiveItem() => _slots[_activeSlot];
    public ItemDefinition GetSlotItem(int index) => _slots[index];
    public int GetActiveSlot() => _activeSlot;
    public bool GetInventoryOpen() => _isOpen;
}

public struct PickupResult
{
    public bool success;
    public ItemDefinition displaced;
}