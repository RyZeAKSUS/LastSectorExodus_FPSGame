using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void Play()
    {
        SceneManager.LoadScene("Map");
    }

    public void Quit()
    {
        Application.Quit();
    }
}