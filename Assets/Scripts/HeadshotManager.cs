using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HeadshotManager : MonoBehaviour
{
    public static HeadshotManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI headshotText;
    public float displayDuration = 1.5f;

    [Header("Crosshair")]
    public Image[] crosshairLines;
    public Color normalColor = Color.white;
    public Color headshotColor = Color.red;
    public float crosshairFlashDuration = 0.3f;

    [Header("Score Headshot")]
    public int headshotScoreBonus = 10;

    private Coroutine _hideCoroutine;
    private Coroutine _crosshairCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (headshotText != null)
        {
            headshotText.gameObject.SetActive(false);
        }
    }

    public void RegisterHeadshot()
    {
        if (headshotText != null)
        {
            headshotText.gameObject.SetActive(true);
            headshotText.text = "HEADSHOT!";

            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
            }
            _hideCoroutine = StartCoroutine(HideTextAfterDelay());
        }

        if (_crosshairCoroutine != null)
        {
            StopCoroutine(_crosshairCoroutine);
        }
        _crosshairCoroutine = StartCoroutine(FlashCrosshair());

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddWaveStreakBonus(headshotScoreBonus);
        }

        if (AdrenalineSystem.Instance != null)
        {
            AdrenalineSystem.Instance.AddAdrenaline(
                AdrenalineSystem.Instance.adrenalinePerKill
            );
        }
    }

    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (headshotText != null)
        {
            headshotText.gameObject.SetActive(false);
        }
    }

    IEnumerator FlashCrosshair()
    {
        SetCrosshairColor(headshotColor);
        yield return new WaitForSeconds(crosshairFlashDuration);
        SetCrosshairColor(normalColor);
    }

    void SetCrosshairColor(Color color)
    {
        if (crosshairLines == null) return;
        foreach (Image line in crosshairLines)
        {
            if (line != null)
            {
                line.color = color;
            }
        }
    }
}