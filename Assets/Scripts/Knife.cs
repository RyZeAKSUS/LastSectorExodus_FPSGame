using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Configuração")]
    public float damage = 40f;
    public float range = 4f;
    public float attackRate = 0.25f;
    public float dashForce = 4f;
    public float dashDuration = 0.1f;

    [Header("Referências")]
    public Camera playerCamera;

    private float _nextAttackTime;
    private CharacterController _cc;
    private bool _isDashing = false;

    void Start()
    {
        _cc = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return;
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing()) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackRate;
            StartCoroutine(AttackWithDash());
        }
    }

    System.Collections.IEnumerator AttackWithDash()
    {
        _isDashing = true;

        float elapsed = 0f;
        Vector3 dashDirection = playerCamera.transform.forward;
        dashDirection.y = 0f;
        dashDirection.Normalize();

        while (elapsed < dashDuration)
        {
            if (_cc != null)
            {
                _cc.Move(dashDirection * dashForce * Time.deltaTime);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        _isDashing = false;

        Attack();
    }

    void Attack()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}