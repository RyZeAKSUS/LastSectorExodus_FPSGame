using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackRate = 1f;

    private NavMeshAgent _agent;
    private float _nextAttackTime;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        _agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackRate;
            Attack();
        }
    }

    void Attack()
    {
        player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        Debug.Log("Inimigo atacou! HP do player: " + player.GetComponent<PlayerHealth>().GetCurrentHealth());
    }

    public void Die()
    {
        FindFirstObjectByType<EnemySpawner>().EnemyDied();
        Destroy(gameObject);
    }
}