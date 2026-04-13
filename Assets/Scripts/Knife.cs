using UnityEngine;
using System.Collections;

public class Knife : MonoBehaviour
{
    [Header("Configuração")]
    public float damage = 40f;
    public float range = 4f;
    public float attackCooldown = 2.5f;
    public float dashForce = 12f;
    public float dashDuration = 0.2f;
    public float invincibilityDuration = 0.45f;

    [Header("Referências")]
    public Camera playerCamera;

    private float _nextAttackTime;
    private CharacterController _cc;
    private PlayerHealth _playerHealth;
    private bool _isInvincible = false;

    void Start()
    {
        _cc = GetComponentInParent<CharacterController>();
        _playerHealth = GetComponentInParent<PlayerHealth>();
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
            _nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(DashAttack());
        }
    }

    IEnumerator DashAttack()
    {
        _isInvincible = true;

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

        Attack(dashDirection);

        float remainingInvincibility = invincibilityDuration - dashDuration;
        if (remainingInvincibility > 0f)
        {
            yield return new WaitForSeconds(remainingInvincibility);
        }

        _isInvincible = false;
    }

    void Attack(Vector3 dashDirection)
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, range);

        foreach (RaycastHit hit in hits)
        {
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    public bool IsInvincible() => _isInvincible;
}