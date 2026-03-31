using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "FPS/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName = "Enemy";
    public float maxHealth = 50f;
    public float moveSpeed = 3.5f;
    public float attackDamage = 10f;
    public float attackRate = 1f;
    public float attackRange = 2f;
}