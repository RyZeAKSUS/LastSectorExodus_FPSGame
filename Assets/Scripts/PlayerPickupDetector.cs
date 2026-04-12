using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerPickupDetector : MonoBehaviour
{
    [Header("Configuração")]
    public float detectionRadius = 2.5f;
    public TextMeshProUGUI interactText;

    private WorldItemPickup _currentPickup;

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

        _currentPickup = FindClosestPickup();

        if (_currentPickup != null)
        {
            UpdateText();

            if (Input.GetKeyDown(KeyCode.F))
            {
                _currentPickup.Pickup();
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

    void UpdateText()
    {
        if (interactText == null || _currentPickup == null || _currentPickup.itemDefinition == null) return;

        interactText.gameObject.SetActive(true);

        if (!_currentPickup.CanPickup())
        {
            interactText.text = "Inventário Cheio";
        }
        else
        {
            interactText.text = "F - Apanhar " + _currentPickup.itemDefinition.itemName;
        }
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
        HideText();
    }
}