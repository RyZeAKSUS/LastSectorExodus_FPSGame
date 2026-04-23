using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public Transform player;
    public bool startActive = false;

    [Header("Boss Settings")]
    public bool isBoss = false;
    public float rageHealthThreshold = 0.5f;
    private bool _rageActivated = false;

    [Header("Patrulha")]
    public float patrolRadius = 8f;
    public float patrolWaitTime = 2f;
    public float detectionRadius = 20f;
    public float losDetectionRadius = 35f;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;
    public AudioClip roarSound;
    public AudioClip[] footstepSounds;

    [Header("Configuração de Sons")]
    public float roarInterval = 8f;
    public float roarIntervalVariance = 4f;
    public float footstepInterval = 0.5f;
    public float footstepVolume = 0.3f;

    private static float _globalRoarCooldown = 0f;
    private static float _globalRoarCooldownDuration = 2f;

    [HideInInspector] public EnemySpawner assignedSpawner;

    private NavMeshAgent _agent;
    private float _nextAttackTime;
    private Animator _animator;
    private bool _isDead = false;
    private bool _isMoving = false;
    private bool _isActive = false;
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
    private float _nextRoarTime = 0f;
    private float _footstepTimer = 0f;

    private Vector3 _spawnPosition;
    private Vector3 _patrolTarget;
    private float _patrolWaitTimer = 0f;
    private bool _isWaiting = false;
    private bool _isChasingPlayer = false;

    private static System.Collections.Generic.Dictionary<EnemySpawner, bool> _spawnerAlertState
        = new System.Collections.Generic.Dictionary<EnemySpawner, bool>();

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
    }

    void Start()
    {
        _spawnPosition = transform.position;
        _patrolTarget = transform.position;

        _animator = null;
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.gameObject.activeInHierarchy)
            {
                Animator anim = child.GetComponent<Animator>();
                if (anim != null)
                {
                    _animator = anim;
                    break;
                }
            }
        }

        if (data != null)
        {
            _agent.speed = data.moveSpeed;
        }

        _isActive = startActive;

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

        if (_hasIdleIndex)
        {
            _animator?.SetInteger("idleIndex", 0);
        }
        if (_hasWalkIndex)
        {
            _animator?.SetInteger("walkIndex", 0);
        }

        if (_isActive)
        {
            _agent.isStopped = false;
        }

        _nextRoarTime = Time.time + Random.Range(roarInterval, roarInterval + roarIntervalVariance);

        if (assignedSpawner != null && IsZoneAlerted())
        {
            _isChasingPlayer = true;
        }
    }

    bool IsZoneAlerted()
    {
        if (assignedSpawner == null) return false;
        return _spawnerAlertState.ContainsKey(assignedSpawner)
            && _spawnerAlertState[assignedSpawner];
    }

    void AlertZone()
    {
        if (assignedSpawner == null) return;
        _spawnerAlertState[assignedSpawner] = true;
        assignedSpawner.OnFirstEnemyAlert();
    }

    void Update()
    {
        if (_isDead || player == null) return;

        if (_globalRoarCooldown > 0f)
        {
            _globalRoarCooldown -= Time.deltaTime;
        }

        if (!_isActive)
        {
            HandleIdleAnimation();
            return;
        }

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

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = CanSeePlayer(distanceToPlayer);

        if (canSeePlayer && !_isChasingPlayer)
        {
            _isChasingPlayer = true;
            AlertZone();
        }

        if (!_isChasingPlayer && IsZoneAlerted())
        {
            _isChasingPlayer = true;
        }

        if (_isChasingPlayer && distanceToPlayer > detectionRadius * 1.5f && !canSeePlayer)
        {
            _isChasingPlayer = false;
            if (assignedSpawner != null)
            {
                _spawnerAlertState[assignedSpawner] = false;
            }
        }

        if (_isChasingPlayer)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        HandleRoar();
    }

    bool CanSeePlayer(float distance)
    {
        if (distance <= detectionRadius)
        {
            return true;
        }

        if (distance <= losDetectionRadius)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            if (Physics.Raycast(transform.position + Vector3.up, directionToPlayer,
                out RaycastHit hit, losDetectionRadius))
            {
                if (hit.transform == player || hit.transform.IsChildOf(player))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void ChasePlayer()
    {
        _agent.SetDestination(player.position);

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= data.attackRange)
        {
            _agent.isStopped = true;
            SetMoving(false);

            if (Time.time >= _nextAttackTime)
            {
                _nextAttackTime = Time.time + data.attackRate;
                Attack();
            }
        }
        else
        {
            _agent.isStopped = false;
            SetMoving(true);
        }

        HandleFootsteps();
    }

    void Patrol()
    {
        if (_isWaiting)
        {
            _patrolWaitTimer -= Time.deltaTime;
            SetMoving(false);
            _agent.isStopped = true;

            if (_patrolWaitTimer <= 0f)
            {
                _isWaiting = false;
                ChooseNewPatrolTarget();
            }
            return;
        }

        float distanceToTarget = Vector3.Distance(transform.position, _patrolTarget);

        if (distanceToTarget < 1f)
        {
            _isWaiting = true;
            _patrolWaitTimer = patrolWaitTime + Random.Range(0f, 2f);
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(_patrolTarget);
        SetMoving(true);
        HandleFootsteps();
    }

    void ChooseNewPatrolTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPoint = _spawnPosition + Random.insideUnitSphere * patrolRadius;
            randomPoint.y = _spawnPosition.y;

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                _patrolTarget = hit.position;
                return;
            }
        }

        _patrolTarget = _spawnPosition;
    }

    void SetMoving(bool moving)
    {
        if (_isMoving == moving) return;
        _isMoving = moving;

        if (moving)
        {
            if (isBoss && _rageActivated && _hasIsRunning)
            {
                _animator?.SetBool("isRunning", true);
                if (_hasRunIndex)
                {
                    _animator?.SetInteger("runIndex", Random.Range(0, 3));
                }
            }
            else
            {
                _animator?.SetBool("isWalking", true);
                if (_hasWalkIndex)
                {
                    _animator?.SetInteger("walkIndex", Random.Range(0, _maxWalkIndex + 1));
                }
            }
        }
        else
        {
            _animator?.SetBool("isWalking", false);
            if (_hasIsRunning)
            {
                _animator?.SetBool("isRunning", false);
            }
        }
    }

    void HandleIdleAnimation()
    {
        _idleTimer += Time.deltaTime;
        if (_idleTimer >= _idleChangeTime)
        {
            _idleTimer = 0f;
            if (_hasIdleIndex)
            {
                _animator?.SetInteger("idleIndex", Random.Range(0, _maxIdleIndex + 1));
            }
        }
    }

    void HandleFootsteps()
    {
        if (footstepSounds == null || footstepSounds.Length == 0) return;
        if (audioSource == null) return;
        if (!_isMoving) return;

        _footstepTimer += Time.deltaTime;
        if (_footstepTimer >= footstepInterval)
        {
            _footstepTimer = 0f;
            AudioClip step = footstepSounds[Random.Range(0, footstepSounds.Length)];
            if (step != null)
            {
                audioSource.PlayOneShot(step, footstepVolume);
            }
        }
    }

    void HandleRoar()
    {
        if (roarSound == null || audioSource == null) return;

        if (Time.time >= _nextRoarTime)
        {
            if (_globalRoarCooldown <= 0f)
            {
                audioSource.PlayOneShot(roarSound);
                _globalRoarCooldown = _globalRoarCooldownDuration;
            }

            _nextRoarTime = Time.time + roarInterval + Random.Range(0f, roarIntervalVariance);
        }
    }

    public void Activate()
    {
        _isActive = true;
        if (_agent != null && _agent.isOnNavMesh)
        {
            _agent.isStopped = false;
        }

        if (IsZoneAlerted())
        {
            _isChasingPlayer = true;
        }
        else
        {
            ChooseNewPatrolTarget();
        }
    }

    void Attack()
    {
        if (_hasAttackIndex)
        {
            _animator?.SetInteger("attackIndex", Random.Range(0, _maxAttackIndex + 1));
        }
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
                _animator?.SetInteger("hitIndex", Random.Range(0, maxHitIndex + 1));
            }
            _animator?.SetTrigger("takehit");

            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }

            _isChasingPlayer = true;
            AlertZone();
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
            _animator?.SetInteger("deathIndex", Random.Range(0, maxDeathIndex + 1));
        }
        _animator?.SetTrigger("death");

        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        GetComponent<EnemyHealth>().AwardScore();

        if (assignedSpawner != null)
        {
            assignedSpawner.EnemyDied();
        }

        Destroy(gameObject, 3f);
    }
}