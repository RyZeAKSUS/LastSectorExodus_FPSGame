using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs por tipo")]
    public GameObject[] scavengerVariants;
    public GameObject[] arachnidVariants;
    public GameObject[] mutantVariants;
    public GameObject[] bossVariants;

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

    void Update()
    {
        if (!activated || _gameOver) return;

        if (_enemiesAlive <= 0 && !_waitingForNextWave)
        {
            if (_currentWave >= totalWaves)
            {
                _gameOver = true;
                FindFirstObjectByType<VictoryMenu>()?.ShowVictory();
                return;
            }
            _waitingForNextWave = true;
            UpdateWaveUI("Próxima wave em " + timeBetweenWaves + " segundos");
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

        int enemiesToSpawn = _currentWave == totalWaves ? 3 : enemiesPerWave + (_currentWave - 1) * 2;
        _enemiesAlive = enemiesToSpawn;

        UpdateWaveUI("Wave " + _currentWave + " / " + totalWaves);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 spawnPos = spawnPoint.position + offset;

            GameObject prefab = ChoosePrefab();
            GameObject enemy = Instantiate(prefab, spawnPos, spawnPoint.rotation);
            EnemyController ctrl = enemy.GetComponent<EnemyController>();
            ctrl.player = player;

            StartCoroutine(ActivateAfterDelay(ctrl, i * 0.2f));
        }
    }

    GameObject ChoosePrefab()
    {
        GameObject[] pool;

        if (_currentWave == 1)
        {
            pool = scavengerVariants;
        }
        else if (_currentWave == 2)
        {
            pool = Random.value < 0.5f ? scavengerVariants : arachnidVariants;
        }
        else if (_currentWave == 3)
        {
            float r = Random.value;
            pool = r < 0.33f ? scavengerVariants : r < 0.66f ? arachnidVariants : mutantVariants;
        }
        else if (_currentWave == 4)
        {
            float r = Random.value;
            pool = r < 0.33f ? scavengerVariants : r < 0.66f ? arachnidVariants : mutantVariants;
        }
        else
        {
            if (Random.value < 0.3f)
            {
                pool = bossVariants;
            }
            else
            {
                float r = Random.value;
                pool = r < 0.33f ? scavengerVariants : r < 0.66f ? arachnidVariants : mutantVariants;
            }
        }

        return pool[Random.Range(0, pool.Length)];
    }

    System.Collections.IEnumerator ActivateAfterDelay(EnemyController ctrl, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ctrl != null) 
        {
            ctrl.Activate();
        }
    }

    void UpdateWaveUI(string text)
    {
        if (waveText != null) 
        {    
            waveText.text = text;
        }
    }

    public void EnemyDied() => _enemiesAlive--;
}