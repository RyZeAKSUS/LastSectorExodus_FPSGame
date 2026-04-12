using UnityEngine;
using TMPro;
using System.Collections;

public class WaveNotification : MonoBehaviour
{
    public static WaveNotification Instance { get; private set; }

    public TextMeshProUGUI notificationText;
    public float displayDuration = 3f;

    private Coroutine _hideCoroutine;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    public void Show(string message)
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = message;
        }

        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }
        _hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    public void ShowPersistent(string message)
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = message;
        }
    }

    public void Hide()
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
        }

        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }
}