using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuração")]
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public Transform player;

    [Header("Waves")]
    public int enemiesPerWave = 4;
    public float timeBetweenWaves = 8f;
    public int totalWaves = 5;

    [Header("UI")]
    public TextMeshProUGUI waveText;

    private int _currentWave = 0;
    private int _enemiesAlive = 0;
    private bool _waitingForNextWave = false;
    private bool _gameOver = false;

    void Start()
    {
        StartWave();
    }

    void Update()
    {
        if (_gameOver) return;

        if (_enemiesAlive <= 0 && !_waitingForNextWave)
        {
            if (_currentWave >= totalWaves)
            {
                _gameOver = true;
                FindFirstObjectByType<VictoryMenu>().ShowVictory();
                return;
            }

            _waitingForNextWave = true;
            UpdateWaveUI("Próxima wave em " + timeBetweenWaves + "s...");
            Invoke(nameof(StartWave), timeBetweenWaves);
        }
    }

    void StartWave()
    {
        _currentWave++;
        _waitingForNextWave = false;

        int enemiesToSpawn = _currentWave == 5 ? 3 : enemiesPerWave + (_currentWave - 1) * 2;
        _enemiesAlive = enemiesToSpawn;

        UpdateWaveUI("Wave " + _currentWave + " / " + totalWaves);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject prefab = ChoosePrefab();
            GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            enemy.GetComponent<EnemyController>().player = player;
        }
    }

    GameObject ChoosePrefab()
    {
        if (_currentWave == 1)
        {
            return enemyPrefabs[0];
        }
        else if (_currentWave == 2)
        {
            return enemyPrefabs[Random.Range(0, 2)];
        }
        else if (_currentWave == 3)
        {
            return enemyPrefabs[Random.Range(0, 3)];
        }
        else if (_currentWave == 4)
        {
            return enemyPrefabs[Random.Range(0, 3)];
        }
        else
        {
            if (Random.value < 0.3f)
                return enemyPrefabs[3];
            return enemyPrefabs[Random.Range(0, 3)];
        }
    }

    void UpdateWaveUI(string text)
    {
        if (waveText != null)
        {
            waveText.text = text;
        }
    }

    public void EnemyDied()
    {
        _enemiesAlive--;
    }
}