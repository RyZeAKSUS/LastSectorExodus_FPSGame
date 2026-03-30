using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public TextMeshProUGUI healthText;

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

    void Die()
    {
        Debug.Log("Player morreu!");
    }

    void UpdateUI()
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + _currentHealth + " / " + maxHealth;
        }
    }
}