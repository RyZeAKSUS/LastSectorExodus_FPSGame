using UnityEngine;

public enum ItemCategory { Weapon, Healing, Food }

[CreateAssetMenu(fileName = "ItemDefinition", menuName = "FPS/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    [Header("Info")]
    public string itemName = "Item";
    public ItemCategory category;
    public Sprite icon;

    [Header("Prefab 3D (aparece na mão)")]
    public GameObject worldPrefab;

    [Header("Pickup (aparece no chão)")]
    public GameObject pickupPrefab;

    [Header("Índice do slot")]
    public int weaponIndex = 0;

    [Header("Efeitos ao usar (cosméticos)")]
    public float healAmount = 0f;
}