using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public Transform player;

    private NavMeshAgent _agent;
    private float _nextAttackTime;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (data != null)
        {
            _agent.speed = data.moveSpeed;
        }
    }

    void Update()
    {
        if (player == null) return;

        _agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= data.attackRange && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + data.attackRate;
            Attack();
        }
    }

    void Attack()
    {
        player.GetComponent<PlayerHealth>().TakeDamage(data.attackDamage);
    }

    public void Die()
    {
        FindFirstObjectByType<EnemySpawner>().EnemyDied();
        Destroy(gameObject);
    }
}