using UnityEngine;
using TMPro;

public class HealthBox : MonoBehaviour
{
    [Header("Configuração")]
    public float healAmount = 50f;
    public float respawnTime = 20f;
    public string interactMessage = "Pressiona F para recuperar vida";

    [Header("Materiais")]
    public Material fullMaterial;
    public Material emptyMaterial;

    [Header("Referências")]
    public TextMeshProUGUI interactText;

    private bool _isAvailable = true;
    private bool _playerNearby = false;
    private MeshRenderer _meshRenderer;
    private PlayerHealth _playerHealth;

    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!_playerNearby) return;

        UpdateInteractText();

        if (_isAvailable && Input.GetKeyDown(KeyCode.F))
        {
            Collect();
        }
    }

    void UpdateInteractText()
    {
        if (interactText == null) return;

        if (_isAvailable)
        {
            interactText.text = interactMessage;
        }
        else
        {
            interactText.text = "Caixa vazia";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = true;
        _playerHealth = other.GetComponent<PlayerHealth>();

        if (interactText != null)
        {
            interactText.gameObject.SetActive(true);
        }

        UpdateInteractText();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerNearby = false;
        _playerHealth = null;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void Collect()
    {
        if (_playerHealth != null)
        {
            _playerHealth.Heal(healAmount);
        }

        _isAvailable = false;
        _meshRenderer.material = emptyMaterial;
        UpdateInteractText();

        Invoke(nameof(Respawn), respawnTime);
    }

    void Respawn()
    {
        _isAvailable = true;
        _meshRenderer.material = fullMaterial;
        UpdateInteractText();
    }
}