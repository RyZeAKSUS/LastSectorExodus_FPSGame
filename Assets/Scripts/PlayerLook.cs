using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 2f;
    public Transform playerBody;

    private float _xRotation = 0f;

    void Update()
    {
        if (VictoryMenu.victoryShowing) return;
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;

        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Rotação vertical (câmara sobe/desce)
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);

        // Rotação horizontal (corpo roda esquerda/direita)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}