using UnityEngine;
using TMPro;

public class AmmoBox : MonoBehaviour
{
    [Header("Configuração")]
    public int ammoAmount = 30;
    public float respawnTime = 15f;
    public string interactMessage = "Pressiona F para apanhar munição";

    [Header("Materiais")]
    public Material fullMaterial;
    public Material emptyMaterial;

    [Header("Referências")]
    public TextMeshProUGUI interactText;

    private bool _isAvailable = true;
    private bool _playerNearby = false;
    private MeshRenderer _meshRenderer;

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
        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void Collect()
    {
        Gun activeGun = FindFirstObjectByType<GunSwitcher>()
                        .weapons[FindFirstObjectByType<GunSwitcher>()._currentWeapon]
                        .GetComponent<Gun>();
        if (activeGun != null)
            activeGun.AddAmmo(ammoAmount);

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