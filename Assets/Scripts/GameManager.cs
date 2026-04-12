using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Todas as combat zones do mapa (por ordem)")]
    public EnemySpawner[] allSpawners;

    private int _completedZones = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void OnZoneCompleted()
    {
        _completedZones++;
        if (_completedZones >= allSpawners.Length)
        {
            FindFirstObjectByType<VictoryMenu>()?.ShowVictory();
        }
    }
}