using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject hud;
    public TextMeshProUGUI currentScoreText;
    public static bool gameIsPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
        
        if (QuickInventory.Instance != null && QuickInventory.Instance.GetInventoryOpen()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {    
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    bool IsInventoryOpen()
    {
        return QuickInventory.Instance != null && QuickInventory.Instance.GetInventoryOpen();
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        hud.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Pause()
    {
        pausePanel.SetActive(true);
        hud.SetActive(false);

        if (currentScoreText != null && ScoreManager.Instance != null)
        {
            currentScoreText.text = "Score: " + ScoreManager.Instance.GetScore();
        }

        Time.timeScale = 0f;
        gameIsPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMainMenu()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}