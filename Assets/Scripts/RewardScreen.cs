using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
            panel.SetActive(false);
    }

    void Start()
    {
        if (ammoButton != null)
        {
            ammoButton.onClick.RemoveAllListeners();
            ammoButton.onClick.AddListener(ChooseAmmo);
        }

        if (healthButton != null)
        {
            healthButton.onClick.RemoveAllListeners();
            healthButton.onClick.AddListener(ChooseHealth);
        }

        if (hungerThirstButton != null)
        {
            hungerThirstButton.onClick.RemoveAllListeners();
            hungerThirstButton.onClick.AddListener(ChooseHungerThirst);
        }

        SetupIcons();
        SetupDescriptions();
    }

    void Update()
    {
        // Tecla de teste
        if (Input.GetKeyDown(KeyCode.P))
        {
            Show(10);
        }
    }

    void SetupIcons()
    {
        if (ammoIcon != null && ammoSprite != null)
            ammoIcon.sprite = ammoSprite;

        if (healthIcon != null && healthSprite != null)
            healthIcon.sprite = healthSprite;

        if (hungerThirstIcon != null && hungerThirstSprite != null)
            hungerThirstIcon.sprite = hungerThirstSprite;
    }

    void SetupDescriptions()
    {
        if (ammoDescription != null)
            ammoDescription.text = "Restaurar Munição\nEnche a reserva de todas as armas";

        if (healthDescription != null)
            healthDescription.text = "Restaurar Vida\nHP totalmente recuperado";

        if (hungerThirstDescription != null)
            hungerThirstDescription.text = "Restaurar Sede e Fome\nAmbas as barras a 100%";
    }

    public void Show(int level)
    {
        _pendingLevel = level;

        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
        }

        if (hud != null)
            hud.SetActive(false);

        if (titleText != null)
            titleText.text = "Nível " + level + " atingido!\nEscolhe a tua recompensa:";

        EnableButtons();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void HideTemporarily()
    {
        if (panel != null)
            panel.SetActive(false);

        ClearSelectedUI();
    }

    public void ShowWithoutPause()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();
        }

        if (hud != null)
            hud.SetActive(false);

        EnableButtons();

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public bool IsShowing()
    {
        return panel != null && panel.activeSelf;
    }

    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);

        if (hud != null)
            hud.SetActive(true);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ClearSelectedUI();
        ForceActiveGunUIUpdate();
    }

    void EnableButtons()
    {
        if (ammoButton != null)
            ammoButton.interactable = true;

        if (healthButton != null)
            healthButton.interactable = true;

        if (hungerThirstButton != null)
            hungerThirstButton.interactable = true;
    }

    void ClearSelectedUI()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
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
            activeGun.ForceUpdateUI();
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
                    gun.AddAmmo(gun.maxReserveAmmo);
            }
        }

        Hide();
    }

    void ChooseHealth()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Heal(playerHealth.maxHealth);

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