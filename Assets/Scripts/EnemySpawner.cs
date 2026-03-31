using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuração")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform player;

    [Header("Waves")]
    public int enemiesPerWave = 3;
    public float timeBetweenWaves = 5f;
    public int totalWaves = 3;

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
            Invoke(nameof(StartWave), timeBetweenWaves);
        }
    }

    void StartWave()
    {
        _currentWave++;
        _waitingForNextWave = false;

        int enemiesToSpawn = enemiesPerWave * _currentWave;
        _enemiesAlive = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            enemy.GetComponent<EnemyController>().player = player;
        }

        Debug.Log("Onda " + _currentWave + " iniciada! Inimigos: " + enemiesToSpawn);
    }

    public void EnemyDied()
    {
        _enemiesAlive--;
    }
}