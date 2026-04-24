using UnityEngine;

public class MenuCameraParallax : MonoBehaviour
{
    [Header("Configuração")]
    public float parallaxStrength = 2f;
    public float smoothSpeed = 3f;
    public float maxAngle = 4f;

    private Quaternion _baseRotation;
    private Quaternion _targetRotation;

    void Start()
    {
        _baseRotation = transform.rotation;
        _targetRotation = _baseRotation;
    }

    void Update()
    {
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        float rotY = Mathf.Clamp(mouseX * parallaxStrength, -maxAngle, maxAngle);
        float rotX = Mathf.Clamp(-mouseY * parallaxStrength, -maxAngle, maxAngle);

        _targetRotation = _baseRotation * Quaternion.Euler(rotX, rotY, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _targetRotation,
            Time.deltaTime * smoothSpeed
        );
    }
}