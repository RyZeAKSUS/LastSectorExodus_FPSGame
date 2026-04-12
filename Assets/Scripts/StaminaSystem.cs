using UnityEngine;
using UnityEngine.UI;

public class StaminaSystem : MonoBehaviour
{
    public static StaminaSystem Instance { get; private set; }

    [Header("Configuração")]
    public float maxStamina = 100f;
    public float depletionRate = 20f;
    public float regenRate = 15f;
    public float crashRegenRate = 7f;
    public float regenDelay = 1.5f;

    [Header("UI")]
    public GameObject staminaPanel;
    public Slider staminaBar;

    private float _currentStamina;
    private float _regenDelayTimer = 0f;
    private bool _isExhausted = false;

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
        _currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }

        if (staminaPanel != null)
        {
            staminaPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
    }

    public bool ConsumeStamina(float deltaTime)
    {
        if (AdrenalineSystem.Instance != null && AdrenalineSystem.Instance.IsActive())
        {
            return true;
        }

        if (_isExhausted) return false;

        _regenDelayTimer = regenDelay;
        _currentStamina -= depletionRate * deltaTime;

        if (_currentStamina <= 0f)
        {
            _currentStamina = 0f;
            _isExhausted = true;
        }

        UpdateUI();
        return !_isExhausted;
    }

    public void RegenStamina(float deltaTime)
    {
        if (_currentStamina >= maxStamina) return;

        _regenDelayTimer -= deltaTime;
        if (_regenDelayTimer > 0f) return;

        float rate = (AdrenalineSystem.Instance != null && AdrenalineSystem.Instance.IsCrashing())
            ? crashRegenRate
            : regenRate;

        _currentStamina += rate * deltaTime;
        _currentStamina = Mathf.Clamp(_currentStamina, 0f, maxStamina);

        if (_isExhausted && _currentStamina >= maxStamina * 0.3f)
        {
            _isExhausted = false;
        }

        UpdateUI();
    }

    public bool IsExhausted() => _isExhausted;

    void UpdateUI()
    {
        if (staminaPanel != null)
        {
            bool shouldShow = _currentStamina < maxStamina;
            staminaPanel.SetActive(shouldShow);
        }

        if (staminaBar != null)
        {
            staminaBar.value = _currentStamina;
        }
    }
}