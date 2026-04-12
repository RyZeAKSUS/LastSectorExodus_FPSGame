using UnityEngine;
using UnityEngine.UI;

public class AdrenalineSystem : MonoBehaviour
{
    public static AdrenalineSystem Instance { get; private set; }

    [Header("Configuração")]
    public float maxAdrenaline = 100f;
    public float adrenalinePerKill = 25f;
    public float depletionRate = 8f;
    public float activeDuration = 6f;
    public float crashDuration = 3f;

    [Header("Modificadores no modo ativo")]
    public float speedMultiplier = 1.3f;
    public float fireRateMultiplier = 1.4f;

    [Header("UI")]
    public GameObject adrenalinePanel;
    public Slider adrenalineBar;

    [Header("Overlay")]
    public AdrenalineOverlay overlay;

    private float _currentAdrenaline = 0f;
    private bool _isActive = false;
    private bool _isCrashing = false;
    private float _activeTimer = 0f;
    private float _crashTimer = 0f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (adrenalinePanel != null)
        {
            adrenalinePanel.SetActive(false);
        }

        if (adrenalineBar != null)
        {
            adrenalineBar.maxValue = maxAdrenaline;
            adrenalineBar.value = 0f;
        }
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;

        if (_isActive)
        {
            _activeTimer -= Time.deltaTime;

            if (_activeTimer <= 0f)
            {
                EndAdrenalineMode();
            }
        }
        else if (_isCrashing)
        {
            _crashTimer -= Time.deltaTime;

            if (_crashTimer <= 0f)
            {
                _isCrashing = false;
            }
        }
        else
        {
            _currentAdrenaline -= depletionRate * Time.deltaTime;
            _currentAdrenaline = Mathf.Clamp(_currentAdrenaline, 0f, maxAdrenaline);
            UpdateUI();
        }
    }

    public void AddAdrenaline(float amount)
    {
        if (_isActive || _isCrashing) return;

        _currentAdrenaline += amount;

        if (_currentAdrenaline >= maxAdrenaline)
        {
            _currentAdrenaline = maxAdrenaline;
            UpdateUI();
            ActivateAdrenalineMode();
            return;
        }

        UpdateUI();
    }

    public void OnPlayerDamaged()
    {
        if (_isActive)
        {
            EndAdrenalineMode();
        }
    }

    void ActivateAdrenalineMode()
    {
        _isActive = true;
        _activeTimer = activeDuration;

        if (overlay != null)
        {
            overlay.StartEffect();
        }
    }

    void EndAdrenalineMode()
    {
        _isActive = false;
        _isCrashing = true;
        _crashTimer = crashDuration;
        _currentAdrenaline = 0f;

        if (overlay != null)
        {
            overlay.StopEffect();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (adrenalinePanel != null)
        {
            bool shouldShow = _currentAdrenaline > 0f || _isActive;
            adrenalinePanel.SetActive(shouldShow);
        }

        if (adrenalineBar != null)
        {
            adrenalineBar.value = _currentAdrenaline;
        }
    }

    public bool IsActive() => _isActive;
    public bool IsCrashing() => _isCrashing;

    public float GetSpeedMultiplier()
    {
        if (_isActive) return speedMultiplier;
        if (_isCrashing) return 0.8f;
        return 1f;
    }

    public float GetFireRateMultiplier()
    {
        if (_isActive) return fireRateMultiplier;
        return 1f;
    }

    public bool HungerThirstPaused() => _isActive;
}