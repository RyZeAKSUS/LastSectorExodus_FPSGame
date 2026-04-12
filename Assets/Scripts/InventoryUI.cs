using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Painel")]
    public GameObject panel;

    [Header("Cruz Esquerda — Armas")]
    [Tooltip("Ordem: 0=Faca(centro) 1=Pistola(cima) 2=Shotgun(direita) 3=Rifle(baixo) 4=Sniper(esquerda)")]
    public Image[] weaponSlotBGs = new Image[5];
    public Image[] weaponSlotIcons = new Image[5];
    public TextMeshProUGUI[] weaponHotkeyLabels = new TextMeshProUGUI[5];

    [Header("Cruz Direita — Cosméticos")]
    [Tooltip("Ordem: 0=Pills(cima) 1=FirstAid(direita) 2=Comida(baixo) 3=Água(esquerda)")]
    public Image[] cosmeticSlotBGs = new Image[4];
    public Image[] cosmeticSlotIcons = new Image[4];
    public TextMeshProUGUI[] cosmeticHotkeyLabels = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] cosmeticStackLabels = new TextMeshProUGUI[4];

    [Header("Definições de itens (para ícones)")]
    public ItemDefinition[] weaponDefinitions = new ItemDefinition[5];
    public ItemDefinition[] cosmeticDefinitions = new ItemDefinition[4];

    [Header("Cores dos slots")]
    public Color colorActive = new Color(1f, 0.8f, 0f, 1f);
    public Color colorOwned = new Color(0.25f, 0.25f, 0.25f, 0.9f);
    public Color colorEmpty = new Color(0.1f, 0.1f, 0.1f, 0.7f);

    private static readonly string[] WeaponKeys = { "1", "2", "3", "4", "5" };
    private static readonly string[] CosmeticKeys = { "6", "7", "8", "9" };

    public void Refresh(InventorySystem inv)
    {
        if (panel != null)
        {
            panel.SetActive(inv.GetIsOpen());
        }

        if (!inv.GetIsOpen()) return;

        int active = inv.GetActiveSlot();

        for (int i = 0; i < 5; i++)
        {
            bool owned = inv.HasWeapon(i);
            bool isActive = (active == i);

            if (weaponSlotBGs[i] != null)
            {
                weaponSlotBGs[i].color = isActive ? colorActive : (owned ? colorOwned : colorEmpty);
            }

            if (weaponSlotIcons[i] != null)
            {
                bool showIcon = owned && weaponDefinitions[i] != null && weaponDefinitions[i].icon != null;
                weaponSlotIcons[i].enabled = showIcon;
                if (showIcon)
                {
                    weaponSlotIcons[i].sprite = weaponDefinitions[i].icon;
                }
            }

            if (weaponHotkeyLabels[i] != null)
            {
                weaponHotkeyLabels[i].text = WeaponKeys[i];
            }
        }

        for (int i = 0; i < 4; i++)
        {
            int count = inv.GetCosmeticCount(i);
            bool hasAny = count > 0;
            bool isActive = (active == i + 5);

            if (cosmeticSlotBGs[i] != null)
            {
                cosmeticSlotBGs[i].color = isActive ? colorActive : (hasAny ? colorOwned : colorEmpty);
            }

            if (cosmeticSlotIcons[i] != null)
            {
                bool showIcon = hasAny && cosmeticDefinitions[i] != null && cosmeticDefinitions[i].icon != null;
                cosmeticSlotIcons[i].enabled = showIcon;
                if (showIcon)
                {
                    cosmeticSlotIcons[i].sprite = cosmeticDefinitions[i].icon;
                }
            }

            if (cosmeticHotkeyLabels[i] != null)
            {
                cosmeticHotkeyLabels[i].text = CosmeticKeys[i];
            }

            if (cosmeticStackLabels[i] != null)
            {
                cosmeticStackLabels[i].gameObject.SetActive(hasAny);
                if (hasAny)
                {
                    cosmeticStackLabels[i].text = count + "/" + InventorySystem.MaxCosmeticStack;
                }
            }
        }
    }
}