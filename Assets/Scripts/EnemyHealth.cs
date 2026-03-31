using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData data;
    public int scoreValue = 10;
    private float _currentHealth;
    private EnemyController _controller;

    void Start()
    {
        _currentHealth = data != null ? data.maxHealth : 50f;
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

    public void AwardScore()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(scoreValue);
        }
    }
}