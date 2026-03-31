using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float jumpHeight = 1.5f;
    public float gravity = -19.6f;

    [Header("Referências")]
    public Transform cameraHolder;

    private CharacterController _cc;
    private Vector3 _velocity;
    private bool _isGrounded;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (VictoryMenu.victoryShowing) return;
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;

        GroundCheck();
        HandleMovement();
        HandleJump();
        ApplyGravity();
    }

    void GroundCheck()
    {
        _isGrounded = _cc.isGrounded;
        if (_isGrounded && _velocity.y < 0f)
        {
            _velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float speed = isRunning ? runSpeed : walkSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        _cc.Move(move * speed * Time.deltaTime);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }
}