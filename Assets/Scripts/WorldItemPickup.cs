using UnityEngine;

public class WorldItemPickup : MonoBehaviour
{
    public ItemDefinition itemDefinition;
    public GameObject indicatorPrefab;

    private Vector3 _myPosition;
    private GameObject _indicator;

    void Start()
    {
        _myPosition = transform.position;

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
        if (itemDefinition == null || InventorySystem.Instance == null) return false;

        if (itemDefinition.category == ItemCategory.Weapon)
        {
            return InventorySystem.Instance.CanPickupWeapon(itemDefinition.weaponIndex);
        }
        else
        {
            return InventorySystem.Instance.CanPickupCosmetic(itemDefinition.weaponIndex);
        }
    }

    public void Pickup()
    {
        if (itemDefinition == null || InventorySystem.Instance == null) return;

        bool success = false;

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
            {
                Destroy(_indicator);
            }
            Destroy(gameObject);
        }
    }
}