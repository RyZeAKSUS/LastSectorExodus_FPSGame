using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WeaponPickup : MonoBehaviour
{
    public int weaponIndex;
    public string weaponName = "Arma";
    public TextMeshProUGUI interactText;
    
    private bool _playerNearby = false;
    private GunSwitcher _gunSwitcher;

    void Start()
    {
        _gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;

        if (!_playerNearby) return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = true;
        if (interactText != null)
        {
            interactText.gameObject.SetActive(true);
            interactText.text = "Pressiona F para apanhar " + weaponName;
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

    void Pickup()
    {
        _gunSwitcher.UnlockWeapon(weaponIndex);
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }
}