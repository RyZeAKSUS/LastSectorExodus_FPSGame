using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickInventoryUI : MonoBehaviour
{
    [Header("4 slots em cruz — ordem: Cima(1), Direita(2), Baixo(3), Esquerda(4)")]
    public Image[] slotBackgrounds = new Image[4];
    public Image[] slotIcons = new Image[4];
    public TextMeshProUGUI[] slotNames = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] slotNumbers = new TextMeshProUGUI[4];

    [Header("Painel")]
    public GameObject panel;

    [Header("Cores")]
    public Color colorActive = new Color(1f, 0.8f, 0f, 1f);
    public Color colorOccupied = new Color(0.25f, 0.25f, 0.25f, 0.9f);
    public Color colorEmpty = new Color(0.1f, 0.1f, 0.1f, 0.7f);

    public void Refresh(ItemDefinition[] slots, int activeSlot, bool isOpen)
    {
        if (panel != null)
        {
            panel.SetActive(isOpen);
        }

        if (!isOpen) return;

        for (int i = 0; i < 4; i++)
        {
            bool hasItem = slots[i] != null;
            bool isActive = i == activeSlot;

            if (slotBackgrounds[i] != null)
            {
                slotBackgrounds[i].color = isActive ? colorActive : (hasItem ? colorOccupied : colorEmpty);
            }

            if (slotIcons[i] != null)
            {
                if (hasItem && slots[i].icon != null)
                {
                    slotIcons[i].enabled = true;
                    slotIcons[i].sprite = slots[i].icon;
                }
                else
                {
                    slotIcons[i].enabled = false;
                }
            }

            if (slotNames[i] != null)
            { 
                slotNames[i].text = hasItem ? slots[i].itemName : "Vazio";
            }

            if (slotNumbers[i] != null)
            {
                slotNumbers[i].text = (i + 1).ToString();
            }
        }
    }
}