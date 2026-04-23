using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("Nome da zona")]
    public string zoneName = "Zona da Igreja";

    [Header("Prefabs por tipo")]
    public GameObject[] scavengerVariants;
    public GameObject[] arachnidVariants;
    public GameObject[] mutantVariants;
    public GameObject[] bossVariants;

    public Transform[] spawnPoints;
    public Transform player;

    [Header("Waves")]
    public int totalWaves = 4;
    public float timeBetweenWaves = 8f;
    public int baseEnemiesPerWave = 4;

    [Header("Drops ao completar a zona")]
    public ItemDefinition[] zoneDropItems;

    [Header("Ativação")]
    public bool activateOnStart = false;
    public bool activated = false;

    private int _currentWave = 0;
    private int _enemiesAlive = 0;
    private bool _waitingForNextWave = false;
    private bool _zoneComplete = false;
    private bool _firstAlertReceived = false;

    void Start()
    {
        if (activateOnStart)
        {
            Activate();
        }
    }

    void Update()
    {
        if (!activated || _zoneComplete) return;

        if (_enemiesAlive <= 0 && !_waitingForNextWave)
        {
            if (_currentWave >= totalWaves)
            {
                _zoneComplete = true;
                OnZoneComplete();
                return;
            }

            _waitingForNextWave = true;
            StartCoroutine(WaveCountdown());
        }
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;
        StartWave();
    }

    IEnumerator WaveCountdown()
    {
        if (LevelSystem.Instance != null && LevelSystem.Instance.HasPendingReward())
        {
            LevelSystem.Instance.TryShowPendingReward();

            while (RewardScreen.Instance != null && RewardScreen.Instance.IsShowing())
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);
        }

        float timer = timeBetweenWaves;

        while (timer > 0f)
        {
            WaveNotification.Instance?.ShowPersistent(
                zoneName + " - Próxima wave em " + Mathf.CeilToInt(timer) + " segundos"
            );
            timer -= Time.deltaTime;
            yield return null;
        }

        _waitingForNextWave = false;
        StartWave();
    }

    void StartWave()
    {
        _currentWave++;

        int enemiesToSpawn;
        if (_currentWave == totalWaves)
        {
            enemiesToSpawn = 3;
        }
        else
        {
            enemiesToSpawn = baseEnemiesPerWave + (_currentWave - 1) * 2;
        }

        _enemiesAlive = enemiesToSpawn;

        if (_currentWave > 1)
        {
            WaveNotification.Instance?.Show(
                zoneName + " - Wave " + _currentWave + "/" + totalWaves
            );
            _firstAlertReceived = false;
        }

        if (_currentWave > 1)
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null && ph.GetCurrentHealth() / ph.maxHealth > 0.75f)
            {
                int bonus = 50 * (_currentWave - 1);
                ScoreManager.Instance?.AddWaveStreakBonus(bonus);
            }
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Length];
            Vector3 offset = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
            Vector3 spawnPos = spawnPoint.position + offset;

            GameObject prefab = ChoosePrefab();
            GameObject enemy = Instantiate(prefab, spawnPos, spawnPoint.rotation);
            EnemyController ctrl = enemy.GetComponent<EnemyController>();

            if (ctrl != null)
            {
                ctrl.player = player;
                ctrl.assignedSpawner = this;
                StartCoroutine(ActivateAfterDelay(ctrl, i * 0.2f));
            }
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
        else
        {
            pool = Random.value < 0.5f ? bossVariants : mutantVariants;
        }

        return pool[Random.Range(0, pool.Length)];
    }

    IEnumerator ActivateAfterDelay(EnemyController ctrl, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (ctrl != null)
        {
            ctrl.Activate();
        }
    }

    void OnZoneComplete()
    {
        WaveNotification.Instance?.Show(zoneName + " - Concluída!");
        SpawnZoneDrops();
        GameManager.Instance?.OnZoneCompleted();
    }

    void SpawnZoneDrops()
    {
        if (zoneDropItems == null || zoneDropItems.Length == 0) return;

        for (int i = 0; i < zoneDropItems.Length; i++)
        {
            ItemDefinition item = zoneDropItems[i];
            if (item == null || item.pickupPrefab == null) continue;

            Transform spawnPoint = spawnPoints[0];
            Vector3 offset = new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            Vector3 spawnPos = spawnPoint.position + offset;

            Vector3 rayOrigin = spawnPos + Vector3.up * 5f;
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 20f,
                Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                spawnPos.y = hit.point.y + 0.1f;
            }

            Instantiate(item.pickupPrefab, spawnPos, item.pickupPrefab.transform.rotation);
        }
    }

    public void OnFirstEnemyAlert()
    {
        if (_firstAlertReceived) return;
        _firstAlertReceived = true;

        WaveNotification.Instance?.Show(
            zoneName + " - Wave " + _currentWave + "/" + totalWaves
        );
    }

    public void EnemyDied() => _enemiesAlive--;
}