using UnityEngine;
using UnityEngine.UI;

public class DamageOverlay : MonoBehaviour
{
    public float flashDuration = 0.3f;
    public float maxAlpha = 0.5f;

    private float _currentAlpha = 0f;
    private bool _isFading = false;
    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
        Color c = _image.color;
        c.a = 0f;
        _image.color = c;
    }

    void Update()
    {
        if (_isFading)
        {
            _currentAlpha -= Time.deltaTime / flashDuration;
            _currentAlpha = Mathf.Clamp(_currentAlpha, 0f, maxAlpha);

            Color c = _image.color;
            c.a = _currentAlpha;
            _image.color = c;

            if (_currentAlpha <= 0f)
            {
                _isFading = false;
            }
        }
    }

    public void Flash()
    {
        _currentAlpha = maxAlpha;
        _isFading = true;
    }
}