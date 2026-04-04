using UnityEngine;

public class CombatZone : MonoBehaviour
{
    public EnemySpawner spawner;
    private bool _triggered = false;

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
}