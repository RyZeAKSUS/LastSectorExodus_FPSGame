using UnityEngine;

public class DEBUG_RewardTest : MonoBehaviour
{
    void Update()
    {
        // Prime P para abrir o ecrã de recompensa
        if (Input.GetKeyDown(KeyCode.P))
        {
            LevelSystem.Instance?.DEBUG_ForceReward();
        }
    }
}