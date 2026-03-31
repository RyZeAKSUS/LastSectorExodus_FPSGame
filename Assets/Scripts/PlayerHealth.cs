using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public Slider healthBar;

    private float _currentHealth;

    void Start()
    {
        _currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
        UpdateUI();

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
}