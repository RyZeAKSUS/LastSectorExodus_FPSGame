using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    public int _currentWeapon = 0;
    private bool[] _unlockedWeapons;

    void Start()
    {
        _unlockedWeapons = new bool[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            _unlockedWeapons[i] = true;
        }

        HideAllWeapons();
    }

    void Update() { }

    public void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }

        _currentWeapon = index;

        Gun gun = weapons[_currentWeapon].GetComponent<Gun>();
        if (gun != null)
        {
            gun.ForceUpdateUI();
            return;
        }

        Knife knife = weapons[_currentWeapon].GetComponent<Knife>();
        if (knife != null)
        {
            Gun firstGun = weapons[1].GetComponent<Gun>();
            if (firstGun != null)
            {
                if (firstGun.ammoText != null)
                {
                    firstGun.ammoText.text = "";
                }
                if (firstGun.fireModeText != null)
                {
                    firstGun.fireModeText.gameObject.SetActive(false);
                }
            }
        }
    }

    public void UnlockWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        _unlockedWeapons[index] = true;
        EquipWeapon(index);
    }

    public void EquipWeaponPublic(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        EquipWeapon(index);
    }

    public void HideAllWeapons()
    {
        foreach (GameObject w in weapons)
        {
            if (w != null)
            {
                w.SetActive(false);
            }
        }
    }
}