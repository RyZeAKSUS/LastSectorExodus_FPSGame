using UnityEngine;

public class PickupIndicator : MonoBehaviour
{
    [Header("Configuração")]
    public float bobHeight = 0.3f;
    public float bobSpeed = 2f;
    public float heightAboveItem = 1.5f;

    private Transform _cameraTransform;
    private Vector3 _basePosition;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _basePosition = transform.localPosition;
    }

    void Update()
    {
        float newY = _basePosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(_basePosition.x, newY, _basePosition.z);

        if (_cameraTransform != null)
        {
            Vector3 dirToCamera = _cameraTransform.position - transform.position;
            dirToCamera.y = 0f;
            if (dirToCamera != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(-dirToCamera);
            }
        }
    }
}