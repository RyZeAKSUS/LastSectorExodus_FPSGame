using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6.5f;
    public float jumpHeight = 1f;
    public float gravity = -19.6f;

    [Header("Dano de Queda")]
    public float fallDamageThreshold = 4f;
    public float fallDamageMultiplier = 2f;

    [Header("Referências")]
    public Transform cameraHolder;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _wasGrounded;
    private float _highestY;
    private bool _isFalling;
    private bool _inWater = false;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _highestY = transform.position.y;
    }

    void Update()
    {
        if (VictoryMenu.victoryShowing) return;
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return;
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return;

        TrackFall();
        GroundCheck();
        HandleMovement();
        HandleJump();
        ApplyGravity();
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
            Debug.Log("Distância de queda: " + fallDistance);

            if (fallDistance > fallDamageThreshold)
            {
                float damage = (fallDistance - fallDamageThreshold) * fallDamageMultiplier;
                Debug.Log("Dano de queda: " + damage);
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
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed;
        if (_inWater)
        {
            speed = 2f;
        }
        else
        {
            speed = isRunning ? runSpeed : walkSpeed;
        }

        Vector3 move = transform.right * x + transform.forward * z;
        _cc.Move(move * speed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded && !_inWater)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
    }
}