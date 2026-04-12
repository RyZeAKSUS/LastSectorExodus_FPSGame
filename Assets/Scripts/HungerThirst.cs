using UnityEngine;
using UnityEngine.UI;

public class HungerThirst : MonoBehaviour
{
    public static HungerThirst Instance { get; private set; }

    [Header("Configuração")]
    public float maxHunger = 100f;
    public float maxThirst = 100f;

    [Tooltip("Unidades por minuto")]
    public float hungerDepletionRate = 3f;

    [Tooltip("Unidades por minuto - desce mais rápido que a fome")]
    public float thirstDepletionRate = 6f;

    [Tooltip("HP drenado por segundo quando a fome chega a zero")]
    public float hungerHpDrainRate = 2f;

    [Tooltip("HP drenado por segundo quando a sede chega a zero")]
    public float thirstHpDrainRate = 5f;

    [Header("UI")]
    public Slider hungerBar;
    public Slider thirstBar;

    private float _hunger;
    private float _thirst;
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
        _hunger = maxHunger;
        _thirst = maxThirst;
        _playerHealth = GetComponent<PlayerHealth>();

        if (hungerBar != null)
        {
            hungerBar.maxValue = maxHunger;
            hungerBar.value = maxHunger;
        }
        if (thirstBar != null)
        {
            thirstBar.maxValue = maxThirst;
            thirstBar.value = maxThirst;
        }
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;

        _hunger -= hungerDepletionRate / 60f * Time.deltaTime;
        _thirst -= thirstDepletionRate / 60f * Time.deltaTime;

        _hunger = Mathf.Clamp(_hunger, 0f, maxHunger);
        _thirst = Mathf.Clamp(_thirst, 0f, maxThirst);

        if (_hunger <= 0f)
        {
            _playerHealth?.TakeDamageRaw(hungerHpDrainRate * Time.deltaTime);
        }

        if (_thirst <= 0f)
        {
            _playerHealth?.TakeDamageRaw(thirstHpDrainRate * Time.deltaTime);
        }

        UpdateUI();
    }

    public void AddHunger(float amount)
    {
        _hunger = Mathf.Clamp(_hunger + amount, 0f, maxHunger);
        UpdateUI();
    }

    public void AddThirst(float amount)
    {
        _thirst = Mathf.Clamp(_thirst + amount, 0f, maxThirst);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (hungerBar != null)
        {
            hungerBar.value = _hunger;
        }
        if (thirstBar != null)
        {
            thirstBar.value = _thirst;
        }
    }
}