using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject crosshair;
    public GameObject hud;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public static bool gameOverShowing = false;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.Stop();
        }

        gameOverPanel.SetActive(true);

        if (hud != null)
        {
            hud.SetActive(false);
        }
        if (crosshair != null)
        {
            crosshair.SetActive(false);
        }

        if (scoreText != null && ScoreManager.Instance != null)
        {
            scoreText.text = "SCORE: " + ScoreManager.Instance.GetScore();
        }
        if (timeText != null && GameTimer.Instance != null)
        {
            timeText.text = "TEMPO: " + GameTimer.Instance.GetFormattedTime();
        }

        Time.timeScale = 0f;
        gameOverShowing = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (gameOverShowing && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
    }

    public void Restart()
    {
        gameOverShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        gameOverShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}