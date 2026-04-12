using UnityEngine;

public class CombatZone : MonoBehaviour
{
    public EnemySpawner spawner;
    public CombatZone nextZone;

    private bool _triggered = false;

    void Start() {}

    void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (!other.CompareTag("Player")) return;

        _triggered = true;

        if (spawner != null)
        {
            spawner.Activate();
        }
    }

    public void Unlock()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    public void OnZoneComplete()
    {
        if (nextZone != null)
        {
            nextZone.Unlock();
        }

        GameManager.Instance?.OnZoneCompleted();
    }
}