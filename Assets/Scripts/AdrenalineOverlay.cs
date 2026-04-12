using UnityEngine;
using UnityEngine.UI;

public class AdrenalineOverlay : MonoBehaviour
{
    [Header("Referências")]
    public Image overlayImage;

    [Header("Configuração")]
    public float pulseSpeed = 2f;
    public float minAlpha = 0.05f;
    public float maxAlpha = 0.15f;

    private bool _isActive = false;
    private float _pulseTimer = 0f;

    void Start()
    {
        if (overlayImage != null)
        {
            SetAlpha(0f);
        }
    }

    void Update()
    {
        if (!_isActive) return;

        _pulseTimer += Time.unscaledDeltaTime * pulseSpeed;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(_pulseTimer) + 1f) / 2f);
        SetAlpha(alpha);
    }

    public void StartEffect()
    {
        _isActive = true;
        _pulseTimer = 0f;
    }

    public void StopEffect()
    {
        _isActive = false;
        SetAlpha(0f);
    }

    void SetAlpha(float alpha)
    {
        if (overlayImage == null) return;

        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}