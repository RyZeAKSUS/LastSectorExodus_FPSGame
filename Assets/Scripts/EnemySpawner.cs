using UnityEngine;
using TMPro;
using System.Collections.Generic;

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

    [Header("Combat Zone")]
    public bool activated = false;

    private int _currentWave = 0;
    private int _enemiesAlive = 0;
    private bool _waitingForNextWave = false;
    private bool _gameOver = false;
    private List<EnemyController> _activeEnemies = new List<EnemyController>();

    void Update()
    {
        if (!activated || _gameOver) return;

        if (_enemiesAlive <= 0 && !_waitingForNextWave)
        {
            if (_currentWave >= totalWaves)
            {
                _gameOver = true;
                FindFirstObjectByType<VictoryMenu>().ShowVictory();
                return;
            }
            _waitingForNextWave = true;
            UpdateWaveUI("Próxima wave em " + timeBetweenWaves + "segundos");
            Invoke(nameof(StartWave), timeBetweenWaves);
        }
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;
        StartWave();
    }

    void StartWave()
    {
        _currentWave++;
        _waitingForNextWave = false;
        _activeEnemies.Clear();

        int enemiesToSpawn = _currentWave == 5 ? 3 : enemiesPerWave + (_currentWave - 1) * 2;
        _enemiesAlive = enemiesToSpawn;

        UpdateWaveUI("Wave " + _currentWave + " / " + totalWaves);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];

            Vector3 randomOffset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 spawnPosition = spawnPoint.position + randomOffset;

            GameObject prefab = ChoosePrefab();
            GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            EnemyController controller = enemy.GetComponent<EnemyController>();
            controller.player = player;
            _activeEnemies.Add(controller);

            StartCoroutine(ActivateAfterDelay(controller, i * 0.2f));
        }
    }

    System.Collections.IEnumerator ActivateAfterDelay(EnemyController controller, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (controller != null)
        {
            controller.Activate();
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
            {
                return enemyPrefabs[3];
            }
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