using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Nome da zona")]
    public string zoneName = "Zona da Igreja";

    [Header("Prefabs por tipo")]
    public GameObject[] scavengerVariants;
    public GameObject[] arachnidVariants;
    public GameObject[] mutantVariants;
    public GameObject[] bossVariants;

    [Header("Tipos permitidos nesta zona")]
    public bool allowScavengers = true;
    public bool allowArachnids = false;
    public bool allowMutants = false;
    public bool allowBosses = false;

    [Header("Boss na última wave")]
    public bool forceBossOnFinalWave = false;
    public int bossesOnFinalWave = 1;

    [Header("Spawn")]
    public Transform[] spawnPoints;
    public Transform player;

    [Header("Waves")]
    public int totalWaves = 4;
    public float timeBetweenWaves = 8f;
    public int baseEnemiesPerWave = 4;
    public int enemiesIncreasePerWave = 2;

    [Header("Drops aleatórios ao completar a zona")]
    public ItemDefinition[] possibleDrops;
    public int minDrops = 1;
    public int maxDrops = 3;

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
        _firstAlertReceived = false;

        int enemiesToSpawn = baseEnemiesPerWave + (_currentWave - 1) * enemiesIncreasePerWave;

        if (_currentWave == totalWaves && forceBossOnFinalWave)
        {
            enemiesToSpawn = Mathf.Max(enemiesToSpawn, bossesOnFinalWave);
        }

        _enemiesAlive = enemiesToSpawn;

        if (_currentWave > 1)
        {
            WaveNotification.Instance?.Show(
                zoneName + " - Wave " + _currentWave + "/" + totalWaves
            );
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

            Vector3 offset = new Vector3(
                Random.Range(-3f, 3f),
                0f,
                Random.Range(-3f, 3f)
            );

            Vector3 spawnPos = spawnPoint.position + offset;

            GameObject prefab = ChoosePrefab(i, enemiesToSpawn);

            if (prefab == null)
            {
                Debug.LogWarning("Spawner " + zoneName + " não tem prefabs válidos para os tipos permitidos.");
                _enemiesAlive--;
                continue;
            }

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

    GameObject ChoosePrefab(int enemyIndex, int enemiesToSpawn)
    {
        if (_currentWave == totalWaves &&
            forceBossOnFinalWave &&
            bossVariants != null &&
            bossVariants.Length > 0 &&
            enemyIndex < bossesOnFinalWave)
        {
            return GetRandomFromArray(bossVariants);
        }

        List<GameObject[]> allowedPools = new List<GameObject[]>();

        if (allowScavengers && scavengerVariants != null && scavengerVariants.Length > 0)
            allowedPools.Add(scavengerVariants);

        if (allowArachnids && arachnidVariants != null && arachnidVariants.Length > 0)
            allowedPools.Add(arachnidVariants);

        if (allowMutants && mutantVariants != null && mutantVariants.Length > 0)
            allowedPools.Add(mutantVariants);

        if (allowBosses && bossVariants != null && bossVariants.Length > 0)
            allowedPools.Add(bossVariants);

        if (allowedPools.Count == 0)
        {
            return null;
        }

        GameObject[] chosenPool = allowedPools[Random.Range(0, allowedPools.Count)];
        return GetRandomFromArray(chosenPool);
    }

    GameObject GetRandomFromArray(GameObject[] array)
    {
        if (array == null || array.Length == 0) return null;
        return array[Random.Range(0, array.Length)];
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
        if (possibleDrops == null || possibleDrops.Length == 0) return;

        int dropCount = Random.Range(minDrops, maxDrops + 1);

        for (int i = 0; i < dropCount; i++)
        {
            ItemDefinition item = possibleDrops[Random.Range(0, possibleDrops.Length)];

            if (item == null || item.pickupPrefab == null) continue;

            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Vector3 offset = new Vector3(
                Random.Range(-3f, 3f),
                0f,
                Random.Range(-3f, 3f)
            );

            Vector3 spawnPos = spawnPoint.position + offset;

            Vector3 rayOrigin = spawnPos + Vector3.up * 5f;

            if (Physics.Raycast(
                rayOrigin,
                Vector3.down,
                out RaycastHit hit,
                20f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
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

    public void EnemyDied()
    {
        _enemiesAlive--;
    }
}