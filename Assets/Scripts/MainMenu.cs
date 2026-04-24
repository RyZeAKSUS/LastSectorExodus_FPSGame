using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    [Header("Painéis")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject controlsPanel;

    [Header("Som ambiente")]
    public AudioSource ambientSource;
    public AudioClip[] ambientClips;
    public AudioSource hornSource;
    public AudioClip hornClip;

    void Start()
    {
        ShowMainPanel();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (hornSource != null && hornClip != null)
        {
            hornSource.PlayOneShot(hornClip);
        }

        if (ambientSource != null && ambientClips != null && ambientClips.Length > 0)
        {
            ambientSource.clip = ambientClips[0];
            ambientSource.loop = true;
            ambientSource.Play();
        }

        ClearSelectedUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if ((settingsPanel != null && settingsPanel.activeSelf) ||
                (controlsPanel != null && controlsPanel.activeSelf))
            {
                ShowMainPanel();
                ClearSelectedUI();
            }
        }
    }

    public void Play()
    {
        Time.timeScale = 1f;
        ClearSelectedUI();
        SceneManager.LoadScene("Map");
    }

    public void OpenSettings()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            settingsPanel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void OpenControls()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            controlsPanel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ClearSelectedUI();
    }

    public void BackToMain()
    {
        ShowMainPanel();
        ClearSelectedUI();
    }

    private void ShowMainPanel()
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(true);
            mainPanel.transform.SetAsLastSibling();
        }

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Quit()
    {
        ClearSelectedUI();
        Application.Quit();
    }

    private void ClearSelectedUI()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}