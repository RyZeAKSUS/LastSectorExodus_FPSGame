using UnityEngine;

public class WorldItemPickup : MonoBehaviour
{
    public ItemDefinition itemDefinition;
    public GameObject indicatorPrefab;

    private GameObject _indicator;
    private bool _isBeingPickedUp = false;

    void Start()
    {
        if (indicatorPrefab != null)
        {
            _indicator = Instantiate(
                indicatorPrefab,
                transform.position + Vector3.up * 1.5f,
                Quaternion.identity
            );

            _indicator.transform.SetParent(transform);
        }
    }

    public bool CanPickup()
    {
        if (itemDefinition == null) return false;
        if (InventorySystem.Instance == null) return false;

        if (itemDefinition.category == ItemCategory.Weapon)
        {
            return InventorySystem.Instance.CanPickupWeapon(itemDefinition.weaponIndex);
        }

        return InventorySystem.Instance.CanPickupCosmetic(itemDefinition.weaponIndex);
    }

    public void Pickup()
    {
        if (_isBeingPickedUp) return;
        if (itemDefinition == null) return;
        if (InventorySystem.Instance == null) return;

        _isBeingPickedUp = true;

        bool success;

        if (itemDefinition.category == ItemCategory.Weapon)
        {
            success = InventorySystem.Instance.TryPickupWeapon(itemDefinition.weaponIndex);
        }
        else
        {
            success = InventorySystem.Instance.TryPickupCosmetic(itemDefinition.weaponIndex);
        }

        if (success)
        {
            if (_indicator != null)
                Destroy(_indicator);

            Destroy(gameObject);
        }
        else
        {
            _isBeingPickedUp = false;
        }
    }

    public string GetPickupName()
    {
        if (itemDefinition == null) return "Item";
        return itemDefinition.itemName;
    }
}