using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 50f;
    private float _currentHealth;
    private EnemyController _controller;

    void Start()
    {
        _currentHealth = maxHealth;
        _controller = GetComponent<EnemyController>();
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0f)
        {
            _controller.Die();
        }
    }
}