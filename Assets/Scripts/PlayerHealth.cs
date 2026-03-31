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
        _damageOverlay = FindFirstObjectByType<DamageOverlay>();
        UpdateUI();
    }

    public void TakeDamage(float amount)
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

    public float GetCurrentHealth()
    {
        return _currentHealth;
    }

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

    public void Heal(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        UpdateUI();
    }
}