using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject hud;
    public TextMeshProUGUI currentScoreText;
    public static bool gameIsPaused = false;

    private bool _rewardWasShowing = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return;

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

    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        WaterZone waterZone = FindFirstObjectByType<WaterZone>();
        bool isDrowning = waterZone != null && waterZone.IsInWater();

        if (!isDrowning)
        {
            hud.SetActive(true);
        }

        if (_rewardWasShowing && RewardScreen.Instance != null)
        {
            _rewardWasShowing = false;
            RewardScreen.Instance.ShowWithoutPause();
        }
    }

    void Pause()
    {
        if (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing())
        {
            _rewardWasShowing = true;
            RewardScreen.Instance.HideTemporarily();
        }
        else
        {
            _rewardWasShowing = false;
        }

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