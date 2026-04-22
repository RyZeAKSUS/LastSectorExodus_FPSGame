using UnityEngine;

public class FPSHandsController : MonoBehaviour
{
    [Header("Referências")]
    public Transform leftShoulder;
    public SkinnedMeshRenderer handsMesh;

    [Header("Posições por arma")]
    public Vector3[] handPositions = new Vector3[5];
    public Vector3[] handRotations = new Vector3[5];

    private int _lastSlot = -1;

    void Start()
    {
        UpdateHands();
    }

    void Update()
    {
        if (InventorySystem.Instance == null) return;

        int slot = InventorySystem.Instance.GetActiveSlot();

        if (slot != _lastSlot)
        {
            _lastSlot = slot;
            UpdateHands();
        }
    }

    void UpdateHands()
    {
        if (InventorySystem.Instance == null) return;

        int slot = InventorySystem.Instance.GetActiveSlot();

        if (slot >= 5)
        {
            if (handsMesh != null)
            {
                handsMesh.enabled = false;
            }
            return;
        }

        if (handsMesh != null)
        {
            handsMesh.enabled = true;
        }

        bool twoHanded = slot == 2 || slot == 3 || slot == 4;

        if (leftShoulder != null)
        {
            leftShoulder.localScale = twoHanded ? Vector3.one : Vector3.zero;
        }

        transform.localPosition = handPositions[slot];
        transform.localRotation = Quaternion.Euler(handRotations[slot]);
    }
}