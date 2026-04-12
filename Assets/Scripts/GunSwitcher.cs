using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    public int _currentWeapon = 0;

    void Start()
    {
        HideAllWeapons();
    }

    public void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }

        _currentWeapon = index;

        ClearAllGunUI();

        Gun gun = weapons[_currentWeapon].GetComponent<Gun>();
        if (gun != null)
        {
            gun.ForceUpdateUI();
        }
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

    public void ClearAllGunUI()
    {
        foreach (GameObject w in weapons)
        {
            if (w == null) continue;

            Gun gun = w.GetComponent<Gun>();
            if (gun == null) continue;

            if (gun.ammoText != null)
            {
                gun.ammoText.text = "";
            }
            if (gun.fireModeText != null)
            {
                gun.fireModeText.gameObject.SetActive(false);
            }
        }
    }
}