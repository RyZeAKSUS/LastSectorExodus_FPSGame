using UnityEngine;

public class SkinRandomizer : MonoBehaviour
{
    public GameObject[] skins;

    void Awake()
    {
        if (skins == null || skins.Length == 0) return;

        int chosen = Random.Range(0, skins.Length);

        for (int i = 0; i < skins.Length; i++)
        {
            if (skins[i] == null) continue;
            skins[i].SetActive(i == chosen);
        }
    }
}