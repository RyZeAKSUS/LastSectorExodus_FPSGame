using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6.5f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1f;
    public float gravity = -19.6f;

    [Header("Agachamento")]
    public float standingHeight = 2f;
    public float crouchHeight = 1.4f;
    public float crouchCameraY = -0.25f;
    public float crouchTransitionSpeed = 8f;

    [Header("Dano de Queda")]
    public float fallDamageThreshold = 4f;
    public float fallDamageMultiplier = 2f;

    [Header("Referências")]
    public Transform cameraHolder;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioSource runAudioSource;
    public AudioClip[] walkSounds;
    public AudioClip[] crouchWalkSounds;
    public AudioClip runSound;
    public AudioClip jumpSound;
    public float walkStepInterval = 0.5f;
    public float runStepInterval = 0.3f;
    public float crouchStepInterval = 0.7f;
    public float walkVolume = 0.4f;
    public float runVolume = 0.7f;
    public float crouchVolume = 0.2f;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _wasGrounded;
    private float _highestY;
    private bool _isFalling;
    private bool _inWater = false;
    private float _stepTimer = 0f;
    private bool _wasRunning = false;
    private bool _isCrouching = false;
    private float _cameraBaseY;
    private Vector3 _standingCenter;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _highestY = transform.position.y;

        _standingCenter = _cc.center;
        _cc.height = standingHeight;

        if (cameraHolder != null)
        {
            _cameraBaseY = cameraHolder.localPosition.y;
        }

        if (runAudioSource != null)
        {
            runAudioSource.clip = runSound;
            runAudioSource.loop = true;
            runAudioSource.volume = runVolume;
            runAudioSource.Stop();
        }
    }

    void Update()
    {
        if (VictoryMenu.victoryShowing) return;
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return;

        TrackFall();
        GroundCheck();
        HandleCrouch();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateCrouchCamera();
    }

    void HandleCrouch()
    {
        if (!_isGrounded) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            _isCrouching = !_isCrouching;
        }

        if (_isCrouching && Input.GetKey(KeyCode.LeftShift))
        {
            _isCrouching = false;
        }

        float targetHeight = _isCrouching ? crouchHeight : standingHeight;
        Vector3 targetCenter = _isCrouching
            ? new Vector3(0f, crouchHeight / 2f, 0f)
            : _standingCenter;

        _cc.height = Mathf.MoveTowards(
            _cc.height,
            targetHeight,
            Time.deltaTime * crouchTransitionSpeed
        );
        _cc.center = Vector3.MoveTowards(
            _cc.center,
            targetCenter,
            Time.deltaTime * crouchTransitionSpeed * 0.5f
        );
    }

    void UpdateCrouchCamera()
    {
        if (cameraHolder == null) return;

        float targetCameraY = _isCrouching
            ? _cameraBaseY + crouchCameraY
            : _cameraBaseY;

        Vector3 camPos = cameraHolder.localPosition;
        camPos.y = Mathf.MoveTowards(
            camPos.y,
            targetCameraY,
            Time.deltaTime * crouchTransitionSpeed
        );
        cameraHolder.localPosition = camPos;
    }

    void TrackFall()
    {
        if (!_isGrounded)
        {
            if (_velocity.y >= 0f)
            {
                _highestY = transform.position.y;
            }

            if (_velocity.y < -5f)
            {
                _isFalling = true;
            }
        }
    }

    void GroundCheck()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = _cc.isGrounded;

        if (_isGrounded && !_wasGrounded && _isFalling && !_inWater)
        {
            float fallDistance = _highestY - transform.position.y;

            if (fallDistance > fallDamageThreshold)
            {
                float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;
                GetComponent<PlayerHealth>().TakeDamage(damage);
            }

            _isFalling = false;
            _highestY = transform.position.y;
        }

        if (_isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }
        if (_isGrounded)
        {
            _isFalling = false;
            _highestY = transform.position.y;
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift) && !_isCrouching;
        bool isMoving = (x != 0f || z != 0f) && _isGrounded && !_inWater;

        float baseSpeed;

        if (_inWater)
        {
            baseSpeed = 2f;
            _stepTimer = 0f;
            StopRunSound();
            StaminaSystem.Instance?.RegenStamina(Time.deltaTime);
        }
        else if (_isCrouching)
        {
            baseSpeed = crouchSpeed;
            StaminaSystem.Instance?.RegenStamina(Time.deltaTime);
        }
        else if (wantsToRun && isMoving)
        {
            bool canRun = StaminaSystem.Instance != null
                ? StaminaSystem.Instance.ConsumeStamina(Time.deltaTime)
                : true;

            baseSpeed = canRun ? runSpeed : walkSpeed;

            if (!canRun)
            {
                StaminaSystem.Instance?.RegenStamina(Time.deltaTime);
            }
        }
        else
        {
            baseSpeed = walkSpeed;
            StaminaSystem.Instance?.RegenStamina(Time.deltaTime);
        }

        float adrenalineMultiplier = AdrenalineSystem.Instance != null
            ? AdrenalineSystem.Instance.GetSpeedMultiplier()
            : 1f;

        float adsMultiplier = ADSSystem.Instance != null
            ? ADSSystem.Instance.GetSpeedMultiplier()
            : 1f;

        float speed = baseSpeed * adrenalineMultiplier * adsMultiplier;

        Vector3 move = transform.right * x + transform.forward * z;
        _cc.Move(move * speed * Time.deltaTime);

        bool isRunning = wantsToRun && baseSpeed >= runSpeed && isMoving;

        if (isMoving)
        {
            if (isRunning)
            {
                if (!_wasRunning)
                {
                    StartRunSound();
                }
                _stepTimer = 0f;
            }
            else
            {
                if (_wasRunning)
                {
                    StopRunSound();
                }

                float interval = _isCrouching ? crouchStepInterval : walkStepInterval;
                _stepTimer += Time.deltaTime;
                if (_stepTimer >= interval)
                {
                    _stepTimer = 0f;
                    PlayWalkSound();
                }
            }
        }
        else
        {
            _stepTimer = 0f;
            if (_wasRunning)
            {
                StopRunSound();
            }
        }

        _wasRunning = isRunning;
    }

    void StartRunSound()
    {
        if (runAudioSource != null && runSound != null && !runAudioSource.isPlaying)
        {
            runAudioSource.Play();
        }
    }

    void StopRunSound()
    {
        if (runAudioSource != null && runAudioSource.isPlaying)
        {
            runAudioSource.Stop();
        }
    }

    void PlayWalkSound()
    {
        if (audioSource == null) return;

        if (_isCrouching && crouchWalkSounds != null && crouchWalkSounds.Length > 0)
        {
            AudioClip step = crouchWalkSounds[Random.Range(0, crouchWalkSounds.Length)];
            if (step != null)
            {
                audioSource.PlayOneShot(step, crouchVolume);
            }
        }
        else if (walkSounds != null && walkSounds.Length > 0)
        {
            AudioClip step = walkSounds[Random.Range(0, walkSounds.Length)];
            if (step != null)
            {
                audioSource.PlayOneShot(step, walkVolume);
            }
        }
    }

    void HandleJump()
    {
        if (_isCrouching) return;

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded && !_inWater)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            StopRunSound();

            if (audioSource != null && jumpSound != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
    }

    void ApplyGravity()
    {
        if (_inWater)
        {
            if (_velocity.y < 0f)
            {
                _velocity.y = Mathf.Lerp(_velocity.y, 0f, Time.deltaTime * 5f);
            }
            else
            {
                _velocity.y = Mathf.Lerp(_velocity.y, 0f, Time.deltaTime * 8f);
            }
        }
        else
        {
            _velocity.y += gravity * Time.deltaTime;
        }

        _cc.Move(_velocity * Time.deltaTime);
    }

    public void SetInWater(bool inWater)
    {
        _inWater = inWater;
        _velocity.y = 0f;
        _isGrounded = false;
        StopRunSound();
    }

    public bool IsCrouching() => _isCrouching;
}