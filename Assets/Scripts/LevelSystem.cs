using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance { get; private set; }

    [Header("Configuração")]
    public int baseXPPerLevel = 100;
    public float xpScalingFactor = 1.5f;
    public float hpBonusPerLevel = 0.1f;

    [Header("UI - canto superior esquerdo")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    private int _currentLevel = 1;
    private float _currentXP = 0f;
    private float _xpToNextLevel;
    private PlayerHealth _playerHealth;

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
        _playerHealth = FindFirstObjectByType<PlayerHealth>();
        _xpToNextLevel = baseXPPerLevel;
        UpdateUI();
    }

    public void AddXP(float amount)
    {
        _currentXP += amount;

        while (_currentXP >= _xpToNextLevel)
        {
            _currentXP -= _xpToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        _currentLevel++;
        _xpToNextLevel = baseXPPerLevel * Mathf.Pow(xpScalingFactor, _currentLevel - 1);

        if (_playerHealth != null)
        {
            _playerHealth.IncreaseMaxHealth(_playerHealth.maxHealth * hpBonusPerLevel);
        }

        WaveNotification.Instance?.Show("Nível " + _currentLevel + "!");
    }

    void UpdateUI()
    {
        if (xpBar != null)
        {
            xpBar.maxValue = _xpToNextLevel;
            xpBar.value = _currentXP;
        }

        if (levelText != null)
        {
            levelText.text = "Nível " + _currentLevel;
        }
    }
}