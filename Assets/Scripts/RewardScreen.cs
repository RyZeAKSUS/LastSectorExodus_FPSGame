using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardScreen : MonoBehaviour
{
    public static RewardScreen Instance { get; private set; }

    [Header("Painel")]
    public GameObject panel;
    public GameObject hud;

    [Header("Título")]
    public TextMeshProUGUI titleText;

    [Header("Opção 1 - Munição")]
    public Button ammoButton;
    public Image ammoIcon;
    public TextMeshProUGUI ammoDescription;
    public Sprite ammoSprite;

    [Header("Opção 2 - Vida")]
    public Button healthButton;
    public Image healthIcon;
    public TextMeshProUGUI healthDescription;
    public Sprite healthSprite;

    [Header("Opção 3 - Fome e Sede")]
    public Button hungerThirstButton;
    public Image hungerThirstIcon;
    public TextMeshProUGUI hungerThirstDescription;
    public Sprite hungerThirstSprite;

    private int _pendingLevel = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    void Start()
    {
        if (ammoButton != null)
        {
            ammoButton.onClick.AddListener(ChooseAmmo);
        }
        if (healthButton != null)
        {
            healthButton.onClick.AddListener(ChooseHealth);
        }
        if (hungerThirstButton != null)
        {
            hungerThirstButton.onClick.AddListener(ChooseHungerThirst);
        }

        SetupIcons();
        SetupDescriptions();
    }

    void SetupIcons()
    {
        if (ammoIcon != null && ammoSprite != null)
        {
            ammoIcon.sprite = ammoSprite;
        }
        if (healthIcon != null && healthSprite != null)
        {
            healthIcon.sprite = healthSprite;
        }
        if (hungerThirstIcon != null && hungerThirstSprite != null)
        {
            hungerThirstIcon.sprite = hungerThirstSprite;
        }
    }

    void SetupDescriptions()
    {
        if (ammoDescription != null)
        {
            ammoDescription.text = "Restaurar Munição\nEnche a reserva de todas as armas";
        }
        if (healthDescription != null)
        {
            healthDescription.text = "Restaurar Vida\nHP totalmente recuperado";
        }
        if (hungerThirstDescription != null)
        {
            hungerThirstDescription.text = "Restaurar Sede e Fome\nAmbas as barras a 100%";
        }
    }

    public void Show(int level)
    {
        _pendingLevel = level;

        if (panel != null)
        {
            panel.SetActive(true);
        }
        if (hud != null)
        {
            hud.SetActive(false);
        }
        if (titleText != null)
        {
            titleText.text = "Nível " + level + " atingido!\nEscolhe a tua recompensa:";
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideTemporarily()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void ShowWithoutPause()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        if (hud != null)
        {
            hud.SetActive(false);
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool IsShowing()
    {
        return panel != null && panel.activeSelf;
    }

    void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        if (hud != null)
        {
            hud.SetActive(true);
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ForceActiveGunUIUpdate();
    }

    void ForceActiveGunUIUpdate()
    {
        if (InventorySystem.Instance == null) return;

        int activeSlot = InventorySystem.Instance.GetActiveSlot();
        if (activeSlot <= 0 || activeSlot > 4) return;

        GunSwitcher gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        if (gunSwitcher == null) return;

        Gun activeGun = gunSwitcher.weapons[activeSlot].GetComponent<Gun>();
        if (activeGun != null)
        {
            activeGun.ForceUpdateUI();
        }
    }

    void ChooseAmmo()
    {
        GunSwitcher gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        if (gunSwitcher != null)
        {
            foreach (GameObject weaponObj in gunSwitcher.weapons)
            {
                if (weaponObj == null) continue;

                Gun gun = weaponObj.GetComponent<Gun>();
                if (gun != null)
                {
                    gun.AddAmmo(gun.maxReserveAmmo);
                }
            }
        }
        Hide();
    }

    void ChooseHealth()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(playerHealth.maxHealth);
        }
        Hide();
    }

    void ChooseHungerThirst()
    {
        if (HungerThirst.Instance != null)
        {
            HungerThirst.Instance.AddHunger(100f);
            HungerThirst.Instance.AddThirst(100f);
        }
        Hide();
    }
}