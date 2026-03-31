using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : MonoBehaviour
{
    public GameObject victoryPanel;
    public static bool victoryShowing = false;

    void Start()
    {
        victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        victoryShowing = true;
        victoryPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMainMenu()
    {
        victoryShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}