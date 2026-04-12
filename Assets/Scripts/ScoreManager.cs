using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI multiplierText;

    [Header("Combo")]
    public float comboWindow = 3.5f;
    public int maxMultiplier = 4;

    private int _score = 0;
    private int _multiplier = 1;
    private int _comboCount = 0;
    private float _lastKillTime = -999f;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UpdateUI();
    }

    void Update()
    {
        if (_multiplier > 1 && Time.time - _lastKillTime > comboWindow)
        {
            ResetCombo();
        }
    }

    public void RegisterKill(int baseScore)
    {
        float timeSinceLast = Time.time - _lastKillTime;

        if (timeSinceLast <= comboWindow)
        {
            _comboCount++;
            _multiplier = Mathf.Min(_comboCount + 1, maxMultiplier);
        }
        else
        {
            _comboCount = 1;
            _multiplier = 1;
        }

        _lastKillTime = Time.time;

        int earned = baseScore * _multiplier;
        _score += earned;

        LevelSystem.Instance?.AddXP(earned);

        UpdateUI();
    }

    public void BreakCombo()
    {
        if (_multiplier > 1)
        {
            ResetCombo();
        }
    }

    public void AddWaveStreakBonus(int bonus)
    {
        _score += bonus;
        LevelSystem.Instance?.AddXP(bonus);
        UpdateUI();
    }

    void ResetCombo()
    {
        _multiplier = 1;
        _comboCount = 0;
        UpdateUI();
    }

    public int GetScore() => _score;
    public int GetMultiplier() => _multiplier;

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + _score;
        }

        if (comboText != null)
        {
            comboText.gameObject.SetActive(_multiplier > 1);
            if (_multiplier > 1)
            {
                comboText.text = "COMBO x" + _comboCount;
            }
        }

        if (multiplierText != null)
        {
            multiplierText.gameObject.SetActive(_multiplier > 1);
            if (_multiplier > 1)
            {
                multiplierText.text = "x" + _multiplier;
            }
        }
    }
}