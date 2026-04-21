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

    public void TakeDamage(float amount, bool isHeadshot)
    {
        _currentHealth -= amount;
        _controller.TakeHit();

        if (isHeadshot)
        {
            HeadshotManager.Instance?.RegisterHeadshot();
        }

        if (_currentHealth <= 0f)
        {
            _controller.Die();
        }
    }

    public void AwardScore()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.RegisterKill(scoreValue);
        }
    }

    public float GetHealthPercent()
    {
        return _currentHealth / data.maxHealth;
    }
}