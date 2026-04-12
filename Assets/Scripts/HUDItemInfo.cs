using UnityEngine;
using TMPro;

public class HUDItemInfo : MonoBehaviour
{
    public static HUDItemInfo Instance { get; private set; }

    [Header("Painel principal")]
    public GameObject panel;

    [Header("Modo Arma")]
    public TextMeshProUGUI ammoLabel;
    public TextMeshProUGUI ammoText;
    public GameObject reloadBarObject;
    public UnityEngine.UI.Slider reloadBar;

    [Header("Modo Cosmético")]
    public TextMeshProUGUI cosmeticCountText;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowGun()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        if (ammoLabel != null)
        {
            ammoLabel.gameObject.SetActive(true);
        }
        if (ammoText != null)
        {
            ammoText.gameObject.SetActive(true);
        }
        if (cosmeticCountText != null)
        {
            cosmeticCountText.gameObject.SetActive(false);
        }
    }

    public void ShowCosmetic(int count, int max)
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
        if (ammoLabel != null)
        {
            ammoLabel.gameObject.SetActive(false);
        }
        if (ammoText != null)
        {
            ammoText.gameObject.SetActive(false);
        }
        if (reloadBarObject != null)
        {
            reloadBarObject.SetActive(false);
        }
        if (cosmeticCountText != null)
        {
            cosmeticCountText.gameObject.SetActive(true);
            cosmeticCountText.text = count + " / " + max;
        }
    }

    public void UpdateCosmeticCount(int count, int max)
    {
        if (cosmeticCountText != null)
        {
            cosmeticCountText.text = count + " / " + max;
        }
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}