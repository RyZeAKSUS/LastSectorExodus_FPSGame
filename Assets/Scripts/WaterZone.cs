using UnityEngine;
using TMPro;

public class WaterZone : MonoBehaviour
{
    [Header("Afogamento")]
    public float timeToDrawn = 10f;

    [Header("Nível da água")]
    public float waterSurfaceY = 0f;

    [Header("Referências UI")]
    public GameObject drowningPanel;
    public TextMeshProUGUI countdownText;

    private PlayerHealth _playerHealth;
    private PlayerMovement _playerMovement;
    private bool _inWater = false;
    private float _drowningTimer;

    void Update()
    {
        if (!_inWater) return;
        if (GameOverMenu.gameOverShowing) return;
        if (PauseMenu.gameIsPaused) return;

        if (_playerMovement != null)
        {
            Transform playerTransform = _playerMovement.transform;
            if (playerTransform.position.y < waterSurfaceY - 0.5f)
            {
                Vector3 pos = playerTransform.position;
                pos.y = waterSurfaceY - 0.5f;
                playerTransform.position = pos;
            }
        }

        _drowningTimer -= Time.deltaTime;
        UpdateCountdown();

        if (_drowningTimer <= 0f)
        {
            ExitWater();
            _playerHealth.TakeDamage(9999f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerHealth = other.GetComponent<PlayerHealth>();
        _playerMovement = other.GetComponent<PlayerMovement>();

        EnterWater();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        ExitWater();
    }

    void EnterWater()
    {
        _inWater = true;
        _drowningTimer = timeToDrawn;

        if (_playerMovement != null)
        {
            _playerMovement.SetInWater(true);
        }

        if (drowningPanel != null)
        {
            drowningPanel.SetActive(true);
        }

        UpdateCountdown();
    }

    void ExitWater()
    {
        _inWater = false;

        if (_playerMovement != null)
        {
            _playerMovement.SetInWater(false);
        }

        if (drowningPanel != null)
        {
            drowningPanel.SetActive(false);
        }
    }

    void UpdateCountdown()
    {
        if (countdownText != null)
        {
            countdownText.text = "AFOGAMENTO EM " + Mathf.CeilToInt(_drowningTimer) + " SEGUNDOS";
        }
    }
}