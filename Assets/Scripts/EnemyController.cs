using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public Transform player;

    [Header("Boss Settings")]
    public bool isBoss = false;
    public float rageHealthThreshold = 0.5f;
    private bool _rageActivated = false;

    private NavMeshAgent _agent;
    private float _nextAttackTime;
    private Animator _animator;
    private bool _isDead = false;
    private bool _isMoving = false;
    private float _idleTimer = 0f;
    private float _idleChangeTime = 4f;
    private int _maxIdleIndex = 2;
    private int _maxAttackIndex = 1;
    private int _maxWalkIndex = 4;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        if (data != null)
        {
            _agent.speed = data.moveSpeed;
        }

        if (isBoss)
        {
            _maxIdleIndex = 2;
            _maxAttackIndex = 3;
            _maxWalkIndex = 3;
        }
        else if (gameObject.name.Contains("Mutant"))
        {
            _maxIdleIndex = 3;
            _maxAttackIndex = 5;
            _maxWalkIndex = 4;
        }
        else if (gameObject.name.Contains("Arachnid"))
        {
            _maxIdleIndex = 0;
            _maxAttackIndex = 0;
            _maxWalkIndex = 4;
        }

        _animator?.SetInteger("idleIndex", 0);
        _animator?.SetInteger("walkIndex", 0);
    }

    void Update()
    {
        if (_isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isBoss && !_rageActivated)
        {
            float healthPercent = GetComponent<EnemyHealth>().GetHealthPercent();
            if (healthPercent <= rageHealthThreshold)
            {
                _rageActivated = true;
                _animator?.SetTrigger("rage");
                _agent.speed = data.moveSpeed * 1.5f;
            }
        }

        if (distance <= data.attackRange)
        {
            _agent.SetDestination(transform.position);

            if (_isMoving)
            {
                _isMoving = false;
                _animator?.SetBool("isWalking", false);
                _animator?.SetBool("isRunning", false);
            }

            if (Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + data.attackRate;
                Attack();
            }
        }
        else
        {
            _agent.SetDestination(player.position);

            if (!_isMoving)
            {
                _isMoving = true;

                if (isBoss && _rageActivated)
                {
                    _animator?.SetBool("isRunning", true);
                    _animator?.SetInteger("runIndex", Random.Range(0, 3));
                }
                else
                {
                    _animator?.SetBool("isWalking", true);
                    _animator?.SetInteger("walkIndex", Random.Range(0, _maxWalkIndex + 1));
                }
            }
        }

        if (!_isMoving)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _idleChangeTime)
            {
                _idleTimer = 0f;
                _animator?.SetInteger("idleIndex", Random.Range(0, _maxIdleIndex + 1));
            }
        }
    }

    void Attack()
    {
        _animator?.SetInteger("attackIndex", Random.Range(0, _maxAttackIndex + 1));
        _animator?.SetTrigger("attack");
        player.GetComponent<PlayerHealth>().TakeDamage(data.attackDamage);
    }

    public void TakeHit()
    {
        if (!_isDead)
        {
            int maxHitIndex = isBoss ? 2 : (gameObject.name.Contains("Mutant") ? 3 : 0);
            _animator?.SetInteger("hitIndex", Random.Range(0, maxHitIndex + 1));
            _animator?.SetTrigger("takehit");
        }
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _agent.SetDestination(transform.position);
        _agent.enabled = false;

        int maxDeathIndex = isBoss ? 2 : (gameObject.name.Contains("Mutant") ? 3 : 0);
        _animator?.SetInteger("deathIndex", Random.Range(0, maxDeathIndex + 1));
        _animator?.SetTrigger("death");

        GetComponent<EnemyHealth>().AwardScore();
        FindFirstObjectByType<EnemySpawner>().EnemyDied();

        Destroy(gameObject, 3f);
    }
}