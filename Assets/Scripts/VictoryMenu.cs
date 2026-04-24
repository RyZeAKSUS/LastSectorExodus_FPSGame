using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryMenu : MonoBehaviour
{
    public static bool victoryShowing = false;

    public GameObject victoryPanel;
    public GameObject crosshair;
    public GameObject hud;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    void Start()
    {
        victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        if (GameTimer.Instance != null)
        {
            GameTimer.Instance.Stop();
        }

        victoryPanel.SetActive(true);
        victoryShowing = true;

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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Retry()
    {
        victoryShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        victoryShowing = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}