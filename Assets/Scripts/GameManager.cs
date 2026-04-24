using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class ZoneBarriers
    {
        public string zoneName;
        public GameObject[] barriersToRemove;
    }

    [Header("Spawners por ordem (sequencial)")]
    public EnemySpawner[] allSpawners;

    [Header("Barreiras a remover por zona concluída")]
    public ZoneBarriers[] zoneBarriers;

    [Header("Weapon pickups controlados por zona")]
    public GameObject[] controlledWeaponPickups;

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

        for (int i = 0; i < controlledWeaponPickups.Length; i++)
        {
            if (controlledWeaponPickups[i] != null)
            {
                controlledWeaponPickups[i].SetActive(false);
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

        int completedZoneIndex = _currentZoneIndex - 1;

        RemoveBarriersForZone(completedZoneIndex);
        ActivateWeaponPickupForZone(completedZoneIndex);
        ActivateNextSpawner();

        if (_completedZones >= allSpawners.Length)
        {
            FindFirstObjectByType<VictoryMenu>()?.ShowVictory();
        }
    }

    void RemoveBarriersForZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= zoneBarriers.Length) return;
        if (zoneBarriers[zoneIndex] == null) return;
        if (zoneBarriers[zoneIndex].barriersToRemove == null) return;

        foreach (GameObject barrier in zoneBarriers[zoneIndex].barriersToRemove)
        {
            if (barrier != null)
            {
                barrier.SetActive(false);
            }
        }
    }

    void ActivateWeaponPickupForZone(int zoneIndex)
    {
        if (zoneIndex < 0 || zoneIndex >= controlledWeaponPickups.Length) return;

        if (controlledWeaponPickups[zoneIndex] != null)
        {
            controlledWeaponPickups[zoneIndex].SetActive(true);
        }
    }

    void ActivateNextSpawner()
    {
        if (_currentZoneIndex < allSpawners.Length && allSpawners[_currentZoneIndex] != null)
        {
            allSpawners[_currentZoneIndex].Activate();
        }
    }
}