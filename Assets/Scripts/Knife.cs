using UnityEngine;

public class Knife : MonoBehaviour
{
    [Header("Configuração")]
    public float damage = 75f;
    public float range = 2.5f;
    public float attackRate = 0.4f;

    [Header("Referências")]
    public Camera playerCamera;

    private float _nextAttackTime;

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
            Attack();
        }
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