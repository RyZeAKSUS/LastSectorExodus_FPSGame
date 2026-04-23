using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Spawners por ordem (sequencial)")]
    public EnemySpawner[] allSpawners;

    private int _currentZoneIndex = 0;
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

    void Start()
    {
        for (int i = 0; i < allSpawners.Length; i++)
        {
            if (allSpawners[i] != null)
            {
                allSpawners[i].activated = false;
            }
        }

        if (allSpawners.Length > 0 && allSpawners[0] != null)
        {
            allSpawners[0].Activate();
        }
    }

    public void OnZoneCompleted()
    {
        _completedZones++;
        _currentZoneIndex++;

        if (_currentZoneIndex < allSpawners.Length && allSpawners[_currentZoneIndex] != null)
        {
            allSpawners[_currentZoneIndex].Activate();
        }

        if (_completedZones >= allSpawners.Length)
        {
            FindFirstObjectByType<VictoryMenu>()?.ShowVictory();
        }
    }
}