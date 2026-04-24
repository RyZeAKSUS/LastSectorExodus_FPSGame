using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;
    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider ambientVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    public TextMeshProUGUI ambientVolumeText;

    [Header("Gráficos")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    [Header("Controlos")]
    public Slider sensitivitySlider;
    public TextMeshProUGUI sensitivityText;

    private Resolution[] _resolutions;
    private bool _initialized = false;

    void OnEnable()
    {
        Initialize();
    }

    void Initialize()
    {
        if (_initialized)
        {
            UpdateTexts();
            return;
        }

        _initialized = true;

        SetupResolutionDropdown();

        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float ambient = PlayerPrefs.GetFloat("AmbientVolume", 1f);
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 2f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(master);

        if (sfxVolumeSlider != null)
            sfxVolumeSlider.SetValueWithoutNotify(sfx);

        if (ambientVolumeSlider != null)
            ambientVolumeSlider.SetValueWithoutNotify(ambient);

        if (sensitivitySlider != null)
            sensitivitySlider.SetValueWithoutNotify(sensitivity);

        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        ApplyVolume("MasterVolume", master);
        ApplyVolume("SFXVolume", sfx);
        ApplyVolume("AmbientVolume", ambient);

        Screen.fullScreen = fullscreen;

        ApplySensitivityToPlayer(sensitivity);

        UpdateTexts();
    }

    void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;

            if (!options.Contains(option))
            {
                options.Add(option);
            }

            if (_resolutions[i].width == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = options.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentIndex);
        resolutionDropdown.RefreshShownValue();
    }

    public void SetMasterVolume(float value)
    {
        ApplyVolume("MasterVolume", value);
        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
        UpdateTexts();
    }

    public void SetSFXVolume(float value)
    {
        ApplyVolume("SFXVolume", value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        UpdateTexts();
    }

    public void SetAmbientVolume(float value)
    {
        ApplyVolume("AmbientVolume", value);
        PlayerPrefs.SetFloat("AmbientVolume", value);
        PlayerPrefs.Save();
        UpdateTexts();
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();

        ApplySensitivityToPlayer(value);
        UpdateTexts();
    }

    public void SetResolution(int index)
    {
        if (_resolutions == null || _resolutions.Length == 0) return;
        if (index < 0 || index >= _resolutions.Length) return;

        Resolution res = _resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    void ApplyVolume(string parameter, float value)
    {
        float dB = value <= 0.0001f ? -80f : Mathf.Log10(value) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }

    void ApplySensitivityToPlayer(float value)
    {
        PlayerLook look = FindFirstObjectByType<PlayerLook>();

        if (look != null)
        {
            look.mouseSensitivity = value;
        }
    }

    void UpdateTexts()
    {
        if (masterVolumeText != null && masterVolumeSlider != null)
            masterVolumeText.text = Mathf.RoundToInt(masterVolumeSlider.value * 100) + "%";

        if (sfxVolumeText != null && sfxVolumeSlider != null)
            sfxVolumeText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100) + "%";

        if (ambientVolumeText != null && ambientVolumeSlider != null)
            ambientVolumeText.text = Mathf.RoundToInt(ambientVolumeSlider.value * 100) + "%";

        if (sensitivityText != null && sensitivitySlider != null)
            sensitivityText.text = sensitivitySlider.value.ToString("0.0");
    }
}