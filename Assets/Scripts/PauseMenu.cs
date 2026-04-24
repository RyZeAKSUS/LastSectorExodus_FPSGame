using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("HUD")]
    public GameObject hud;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;

    public static bool gameIsPaused = false;

    private bool _rewardWasShowing = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        gameIsPaused = false;
    }

    void Update()
    {
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;
        if (InventorySystem.Instance != null && InventorySystem.Instance.GetIsOpen()) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSubPanel();
                return;
            }

            if (controlsPanel != null && controlsPanel.activeSelf)
            {
                CloseSubPanel();
                return;
            }

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
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        WaterZone waterZone = FindFirstObjectByType<WaterZone>();
        bool isDrowning = waterZone != null && waterZone.IsInWater();

        if (hud != null && !isDrowning)
            hud.SetActive(true);

        Time.timeScale = 1f;
        gameIsPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ClearSelectedUI();

        if (_rewardWasShowing && RewardScreen.Instance != null)
        {
            _rewardWasShowing = false;
            RewardScreen.Instance.ShowWithoutPause();
        }
    }

    public void Pause()
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

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (hud != null)
            hud.SetActive(false);

        if (scoreText != null && ScoreManager.Instance != null)
            scoreText.text = "SCORE: " + ScoreManager.Instance.GetScore();

        if (timeText != null && GameTimer.Instance != null)
            timeText.text = "TEMPO: " + GameTimer.Instance.GetFormattedTime();

        Time.timeScale = 0f;
        gameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void OpenSettings()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void OpenControls()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void CloseSubPanel()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (pausePanel != null)
            pausePanel.SetActive(true);

        Time.timeScale = 0f;
        gameIsPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        gameIsPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    private void ClearSelectedUI()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }
}