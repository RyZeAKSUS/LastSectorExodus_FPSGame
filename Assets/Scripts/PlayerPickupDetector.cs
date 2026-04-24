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
        if (ShouldBlockPickup())
        {
            ClearAndHide();
            return;
        }

        _currentPickup = FindClosestPickup();
        _currentAmmoBox = FindClosestAmmoBox();

        if (_currentPickup != null)
        {
            bool canPickup = _currentPickup.CanPickup();

            ShowText(
                canPickup
                    ? "F - Apanhar " + _currentPickup.GetPickupName()
                    : "Inventário Cheio"
            );

            if (Input.GetKeyDown(KeyCode.F) && canPickup)
            {
                _currentPickup.Pickup();
            }

            return;
        }

        if (_currentAmmoBox != null)
        {
            HandleAmmoBox();
            return;
        }

        HideText();
    }

    bool ShouldBlockPickup()
    {
        if (PauseMenu.gameIsPaused) return true;
        if (GameOverMenu.gameOverShowing) return true;
        if (VictoryMenu.victoryShowing) return true;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return true;
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return true;

        return false;
    }

    void HandleAmmoBox()
    {
        if (InventorySystem.Instance == null)
        {
            HideText();
            return;
        }

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
                pickup = col.GetComponentInParent<WorldItemPickup>();

            if (pickup == null) continue;
            if (seen.Contains(pickup)) continue;

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
            if (box == null) continue;

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
            interactText.gameObject.SetActive(false);
    }

    void ClearAndHide()
    {
        _currentPickup = null;
        _currentAmmoBox = null;
        HideText();
    }
}