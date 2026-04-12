using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerPickupDetector : MonoBehaviour
{
    [Header("Configuração")]
    public float detectionRadius = 2.5f;
    public TextMeshProUGUI interactText;

    private WorldItemPickup _currentPickup;
    private AmmoBox _currentAmmoBox;

    void Update()
    {
        if (PauseMenu.gameIsPaused)
        {
            ClearAndHide();
            return;
        }
        if (GameOverMenu.gameOverShowing)
        {
            ClearAndHide();
            return;
        }
        if (VictoryMenu.victoryShowing)
        {
            ClearAndHide();
            return;
        }
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen())
        {
            ClearAndHide();
            return;
        }
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing())
        {
            ClearAndHide();
            return;
        }

        _currentPickup = FindClosestPickup();
        _currentAmmoBox = FindClosestAmmoBox();

        if (_currentPickup != null)
        {
            ShowText(
                !_currentPickup.CanPickup()
                    ? "Inventário Cheio"
                    : "F - Apanhar " + _currentPickup.itemDefinition.itemName
            );

            if (Input.GetKeyDown(KeyCode.F))
            {
                _currentPickup.Pickup();
            }
        }
        else if (_currentAmmoBox != null)
        {
            if (!_currentAmmoBox.IsAvailable())
            {
                ShowText("Caixa vazia");
            }
            else
            {
                int activeSlot = InventorySystem.Instance.GetActiveSlot();
                if (activeSlot <= 0 || activeSlot > 4)
                {
                    ShowText("Equipa uma arma primeiro");
                }
                else if (_currentAmmoBox.IsAmmoFull())
                {
                    ShowText("Munição cheia");
                }
                else
                {
                    ShowText("F - Apanhar munição");
                }
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (_currentAmmoBox.IsAvailable() && !_currentAmmoBox.IsAmmoFull())
                {
                    _currentAmmoBox.Collect();
                }
            }
        }
        else
        {
            HideText();
        }
    }

    WorldItemPickup FindClosestPickup()
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            ~0,
            QueryTriggerInteraction.Collide
        );

        WorldItemPickup closest = null;
        float closestDist = float.MaxValue;
        HashSet<WorldItemPickup> seen = new HashSet<WorldItemPickup>();

        foreach (Collider col in colliders)
        {
            WorldItemPickup pickup = col.GetComponent<WorldItemPickup>();
            if (pickup == null)
            {
                pickup = col.GetComponentInParent<WorldItemPickup>();
            }
            if (pickup == null || seen.Contains(pickup)) continue;

            seen.Add(pickup);

            float dist = Vector3.Distance(transform.position, pickup.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = pickup;
            }
        }

        return closest;
    }

    AmmoBox FindClosestAmmoBox()
    {
        AmmoBox[] allBoxes = FindObjectsByType<AmmoBox>(FindObjectsSortMode.None);

        AmmoBox closest = null;
        float closestDist = float.MaxValue;

        foreach (AmmoBox box in allBoxes)
        {
            float dist = Vector3.Distance(transform.position, box.transform.position);
            if (dist < detectionRadius && dist < closestDist)
            {
                closestDist = dist;
                closest = box;
            }
        }

        return closest;
    }

    void ShowText(string message)
    {
        if (interactText == null) return;
        interactText.gameObject.SetActive(true);
        interactText.text = message;
    }

    void HideText()
    {
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void ClearAndHide()
    {
        _currentPickup = null;
        _currentAmmoBox = null;
        HideText();
    }
}