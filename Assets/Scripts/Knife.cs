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

    [Header("Animação de Corte")]
    public float slashRotationAmount = 60f;
    public float slashSpeed = 20f;
    public float slashReturnSpeed = 10f;

    [Header("Referências")]
    public Camera playerCamera;

    [Header("Sons")]
    public AudioSource audioSource;
    public AudioClip swishSound;

    [Header("Trail")]
    public TrailRenderer trailRenderer;

    [Header("Wall Check")]
    public float wallCheckDistance = 0.5f;

    private float _nextAttackTime;
    private CharacterController _cc;
    private bool _isInvincible = false;
    private bool _isAttacking = false;
    private Quaternion _originalRotation;

    void Start()
    {
        _cc = GetComponentInParent<CharacterController>();

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }
    }

    void OnEnable()
    {
        _isAttacking = false;
        _isInvincible = false;
        StopAllCoroutines();

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }

        transform.localRotation = Quaternion.identity;
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
        if (WeaponWallCheck.IsWeaponLowered) return;

        if (Input.GetButtonDown("Fire1") && Time.time >= _nextAttackTime && !_isAttacking)
        {
            _nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(DashAttack());
        }
    }

    IEnumerator DashAttack()
    {
        _isAttacking = true;
        _isInvincible = true;

        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.SetSwapLocked(true);
        }

        if (audioSource != null && swishSound != null)
        {
            audioSource.PlayOneShot(swishSound);
        }

        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;
        }

        _originalRotation = transform.localRotation;
        Quaternion slashRotation = _originalRotation * Quaternion.Euler(0f, 0f, slashRotationAmount);

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

            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                slashRotation,
                Time.deltaTime * slashSpeed
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        Attack();

        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
        }

        float returnElapsed = 0f;
        float returnDuration = 0.15f;
        while (returnElapsed < returnDuration)
        {
            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                _originalRotation,
                Time.deltaTime * slashReturnSpeed
            );
            returnElapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = _originalRotation;

        float remainingInvincibility = invincibilityDuration - dashDuration;
        if (remainingInvincibility > 0f)
        {
            yield return new WaitForSeconds(remainingInvincibility);
        }

        _isInvincible = false;
        _isAttacking = false;

        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.SetSwapLocked(false);
        }
    }

    void Attack()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, range);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damage, false);
            }
        }
    }

    public bool IsInvincible() => _isInvincible;
    public bool IsAttacking() => _isAttacking;
}