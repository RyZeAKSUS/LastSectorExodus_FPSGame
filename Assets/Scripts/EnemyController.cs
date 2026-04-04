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
    private int _maxIdleIndex = 0;
    private int _maxAttackIndex = 0;
    private int _maxWalkIndex = 0;
    private bool _hasIsRunning = false;
    private bool _hasHitIndex = false;
    private bool _hasDeathIndex = false;
    private bool _hasIdleIndex = false;
    private bool _hasWalkIndex = false;
    private bool _hasAttackIndex = false;
    private bool _hasRunIndex = false;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        if (data != null)
        {
            _agent.speed = data.moveSpeed;
        }

        if (_animator != null)
        {
            foreach (AnimatorControllerParameter p in _animator.parameters)
            {
                switch (p.name)
                {
                    case "isRunning": _hasIsRunning = true; break;
                    case "hitIndex": _hasHitIndex = true; break;
                    case "deathIndex": _hasDeathIndex = true; break;
                    case "idleIndex": _hasIdleIndex = true; break;
                    case "walkIndex": _hasWalkIndex = true; break;
                    case "attackIndex": _hasAttackIndex = true; break;
                    case "runIndex": _hasRunIndex = true; break;
                }
            }
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
            _maxAttackIndex = 4;
            _maxWalkIndex = 5;
        }
        else if (gameObject.name.Contains("Arachnid"))
        {
            _maxIdleIndex = 0;
            _maxAttackIndex = 0;
            _maxWalkIndex = 4;
        }
        else
        {
            _maxIdleIndex = 2;
            _maxAttackIndex = 1;
            _maxWalkIndex = 3;
        }

        if (_hasIdleIndex) _animator.SetInteger("idleIndex", 0);
        if (_hasWalkIndex) _animator.SetInteger("walkIndex", 0);
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
                if (_hasIsRunning) _animator.SetBool("isRunning", false);
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

                if (isBoss && _rageActivated && _hasIsRunning)
                {
                    _animator.SetBool("isRunning", true);
                    if (_hasRunIndex) _animator.SetInteger("runIndex", Random.Range(0, 3));
                }
                else
                {
                    _animator?.SetBool("isWalking", true);
                    if (_hasWalkIndex) _animator.SetInteger("walkIndex", Random.Range(0, _maxWalkIndex + 1));
                }
            }
        }

        if (!_isMoving)
        {
            _idleTimer += Time.deltaTime;
            if (_idleTimer >= _idleChangeTime)
            {
                _idleTimer = 0f;
                if (_hasIdleIndex) _animator.SetInteger("idleIndex", Random.Range(0, _maxIdleIndex + 1));
            }
        }
    }

    void Attack()
    {
        if (_hasAttackIndex) _animator.SetInteger("attackIndex", Random.Range(0, _maxAttackIndex + 1));
        _animator?.SetTrigger("attack");
        player.GetComponent<PlayerHealth>().TakeDamage(data.attackDamage);
    }

    public void TakeHit()
    {
        if (!_isDead)
        {
            if (_hasHitIndex)
            {
                int maxHitIndex = isBoss ? 2 : (gameObject.name.Contains("Mutant") ? 3 : 0);
                _animator.SetInteger("hitIndex", Random.Range(0, maxHitIndex + 1));
            }
            _animator?.SetTrigger("takehit");
        }
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        _agent.SetDestination(transform.position);
        _agent.enabled = false;

        if (_hasDeathIndex)
        {
            int maxDeathIndex = isBoss ? 2 : (gameObject.name.Contains("Mutant") ? 3 : 0);
            _animator.SetInteger("deathIndex", Random.Range(0, maxDeathIndex + 1));
        }
        _animator?.SetTrigger("death");

        GetComponent<EnemyHealth>().AwardScore();

        EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.EnemyDied();
        }

        Destroy(gameObject, 3f);
    }
}