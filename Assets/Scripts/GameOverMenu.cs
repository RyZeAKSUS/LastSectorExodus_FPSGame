using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject hud;
    public TextMeshProUGUI finalScoreText;
    public static bool gameOverShowing = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverShowing = true;
        gameOverPanel.SetActive(true);
        hud.SetActive(false);

        if (finalScoreText != null && ScoreManager.Instance != null)
        {
            finalScoreText.text = "Score Final: " + ScoreManager.Instance.GetScore();
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        gameOverShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMainMenu()
    {
        gameOverShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}