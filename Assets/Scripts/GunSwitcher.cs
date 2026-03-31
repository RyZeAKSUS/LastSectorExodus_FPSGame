using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    public GameObject[] weapons;
    public int _currentWeapon = 0;
    private bool[] _unlockedWeapons;

    void Start()
    {
        _unlockedWeapons = new bool[weapons.Length];
        _unlockedWeapons[0] = true;
        EquipWeapon(0);
    }

    void Update()
    {
        if (PauseMenu.gameIsPaused) return;
        if (GameOverMenu.gameOverShowing) return;
        if (VictoryMenu.victoryShowing) return;

        HandleScrollSwitch();
        HandleKeySwitch();
    }

    void HandleScrollSwitch()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            int next = (_currentWeapon + 1) % weapons.Length;
            if (_unlockedWeapons[next]) EquipWeapon(next);
        }
        else if (scroll < 0f)
        {
            int prev = (_currentWeapon - 1 + weapons.Length) % weapons.Length;
            if (_unlockedWeapons[prev]) EquipWeapon(prev);
        }
    }

    void HandleKeySwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && _unlockedWeapons[0]) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && _unlockedWeapons[1]) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && _unlockedWeapons[2]) EquipWeapon(2);
    }

    void EquipWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }

        _currentWeapon = index;

        // Atualizar o AmmoText para a arma equipada
        Gun gun = weapons[_currentWeapon].GetComponent<Gun>();
        if (gun != null)
        {
            gun.ForceUpdateUI();
        }
    }

    public void UnlockWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        _unlockedWeapons[index] = true;
        EquipWeapon(index);
    }
}