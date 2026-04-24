using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    private float _elapsedTime = 0f;
    private bool _running = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _running = true;
    }

    void Update()
    {
        if (!_running) return;
        if (PauseMenu.gameIsPaused) return;
        _elapsedTime += Time.deltaTime;
    }

    public void Stop() => _running = false;

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}