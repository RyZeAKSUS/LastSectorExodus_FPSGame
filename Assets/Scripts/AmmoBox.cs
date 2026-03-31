using UnityEngine;
using TMPro;

public class AmmoBox : MonoBehaviour
{
    [Header("Configuração")]
    public int ammoAmmount = 30;
    public float respawnTime = 15f;
    public string interactMessage = "Pressiona F para apanhar munição";

    [Header("Referências")]
    public TextMeshProUGUI interactText;

    private bool _isAvailable = true;
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
        if (!_isAvailable) return;

        if (interactText != null && interactText.gameObject.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                Collect();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isAvailable) return;
        if (other.CompareTag("Player")) return;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(true);
            interactText.text = interactMessage;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }
    }

    void Collect()
    {
        Gun gun = FindFirstObjectByType<Gun>();
        if (gun != null)
        {
            gun.AddAmmo(ammoAmmount);
        }

        _isAvailable = false;
        _meshRenderer.enabled = false;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        Invoke(nameof(Respawn), respawnTime);
    }

    void Respawn()
    {
        _isAvailable = true;
        _meshRenderer.enabled = true;
    }
}