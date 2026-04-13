using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public Slider healthBar;

    private float _currentHealth;
    private DamageOverlay _damageOverlay;

    void Start()
    {
        _currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = maxHealth;
        }
        _damageOverlay = FindFirstObjectByType<DamageOverlay>();
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        Knife knife = FindFirstObjectByType<Knife>();
        if (knife != null && knife.IsInvincible()) return;

        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        UpdateUI();

        if (_damageOverlay != null)
        {
            _damageOverlay.Flash();
        }

        ScoreManager.Instance?.BreakCombo();
        AdrenalineSystem.Instance?.OnPlayerDamaged();

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    public void TakeDamageRaw(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        UpdateUI();

        if (_damageOverlay != null)
        {
            _damageOverlay.Flash();
        }

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        UpdateUI();
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        _currentHealth += amount;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
        }
        UpdateUI();
    }

    public float GetCurrentHealth() => _currentHealth;

    void Die()
    {
        FindFirstObjectByType<GameOverMenu>().ShowGameOver();
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.value = _currentHealth;
        }
    }
}