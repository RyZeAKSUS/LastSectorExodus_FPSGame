using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class VictoryMenu : MonoBehaviour
{
    public GameObject victoryPanel;
    public GameObject hud;
    public TextMeshProUGUI finalScoreText;
    public static bool victoryShowing = false;

    void Start()
    {
        victoryPanel.SetActive(false);
    }

    public void ShowVictory()
    {
        victoryShowing = true;
        victoryPanel.SetActive(true);
        hud.SetActive(false);

        if (finalScoreText != null && ScoreManager.Instance != null)
        {
            finalScoreText.text = "Score Final: " + ScoreManager.Instance.GetScore();
        }

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