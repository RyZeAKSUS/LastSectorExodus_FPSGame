using UnityEngine;
using TMPro;
using Unity.Mathematics;

public class WorldItemPickup : MonoBehaviour
{
    public ItemDefinition itemDefinition;
    public TextMeshProUGUI interactText;

    private bool _playerNearby = false;

    void Start()
    {
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!_playerNearby) return;
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (QuickInventory.Instance != null && QuickInventory.Instance.GetInventoryOpen()) return;

        UpdatePromptText();

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryPickup();
        }
    }

    void UpdatePromptText()
    {
        if (interactText == null || itemDefinition == null) return;

        bool isFull = QuickInventory.Instance != null && QuickInventory.Instance.InventoryFull();

        if (isFull)
        {
            ItemDefinition activeItem = QuickInventory.Instance.GetActiveItem();
            string activeName = activeItem != null ? activeItem.itemName : "item atual";
            interactText.text = "F - Trocar " + activeName + " por " + itemDefinition.itemName;
        }
        else
        {
            interactText.text = "F - Apanhar " + itemDefinition.itemName;
        }
    }

    void TryPickup()
    {
        if (itemDefinition == null) return;
        if (QuickInventory.Instance == null) return;

        PickupResult result = QuickInventory.Instance.TryPickup(itemDefinition, transform.position);

        if (result.success)
        {
            if (result.displaced != null && result.displaced.pickupPrefab != null)
            {
                Instantiate(result.displaced.pickupPrefab, transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = true;
        if (interactText != null)
        {
            interactText.gameObject.SetActive(true);
            UpdatePromptText();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = false;
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }
}