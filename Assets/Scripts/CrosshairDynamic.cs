using UnityEngine;
using UnityEngine.UI;

public class CrosshairDynamic : MonoBehaviour
{
    [Header("Referências")]
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    [Header("Configuração")]
    public float baseSpread = 15f;
    public float maxSpread = 25f;
    public float expandSpeed = 15f;
    public float shrinkSpeed = 8f;

    private float _currentSpread;
    private Gun _activeGun;
    private GunSwitcher _gunSwitcher;

    void Start()
    {
        _currentSpread = baseSpread;
        _gunSwitcher = FindFirstObjectByType<GunSwitcher>();
        UpdatePositions();
    }

    void Update()
    {
        UpdateActiveGun();
        HandleSpread();
        UpdatePositions();
    }

    void UpdateActiveGun()
    {
        if (_gunSwitcher == null) return;
        GameObject currentWeapon = _gunSwitcher.weapons[_gunSwitcher._currentWeapon];
        _activeGun = currentWeapon.GetComponent<Gun>();
    }

    void HandleSpread()
    {
        bool isShooting = Input.GetButton("Fire1") && _activeGun != null;

        if (isShooting)
        {
            _currentSpread = Mathf.Lerp(_currentSpread, maxSpread, Time.deltaTime * expandSpeed);
        }
        else
        {
            _currentSpread = Mathf.Lerp(_currentSpread, baseSpread, Time.deltaTime * shrinkSpeed);
        }
    }

    void UpdatePositions()
    {
        if (top != null) top.anchoredPosition = new Vector2(0, _currentSpread);
        if (bottom != null) bottom.anchoredPosition = new Vector2(0, -_currentSpread);
        if (left != null) left.anchoredPosition = new Vector2(-_currentSpread, 0);
        if (right != null) right.anchoredPosition = new Vector2(_currentSpread, 0);
    }

    public void SetAiming(bool isAiming)
    {
        if (isAiming)
        {
            _currentSpread = Mathf.Lerp(_currentSpread, baseSpread * 0.3f, Time.deltaTime * 15f);
        }
    }
}