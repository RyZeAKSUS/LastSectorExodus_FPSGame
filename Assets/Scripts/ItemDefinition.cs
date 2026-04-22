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

    [Header("Efeitos ao usar")]
    public float healAmount = 0f;
    public float hungerAmount = 0f;
    public float thirstAmount = 0f;

    [Header("Posição na mão (apenas cosméticos)")]
    public Vector3 handPosition = new Vector3(0.3f, -0.2f, 0.5f);
    public Vector3 handRotation = new Vector3(0f, 180f, 0f);
    public Vector3 handScale = new Vector3(0.3f, 0.3f, 0.3f);
}