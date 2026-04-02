using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Display Settings")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Text refreshRateText;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    #region pritvate fields
    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private const string MusicKey = "MusicVolume";
    private const string SFXKey = "SFXVolume";
    private const string FullscreenKey = "Fullscreen";
    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";

    #endregion

    [System.Obsolete]
    void Start()
    {
        // Resolution setup
        SetupResolutions();

        // Update refresh rate display
        if (refreshRateText != null)
            refreshRateText.text = $"{Screen.currentResolution.refreshRate}Hz";

        LoadSettings();
    }

    #region Audio Sliders
    public void SetVolumeMusic(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat(MusicKey, volume);
    }

    public void SetVolumeSFX(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);

        PlayerPrefs.SetFloat(SFXKey, volume);
    }
    #endregion
    
    #region Display Settings

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreenMode = isFullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        PlayerPrefs.SetInt(FullscreenKey, isFullScreen ? 1 : 0);
    }
    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        // Just add all unique width x height resolutions
        HashSet<string> addedResolutions = new HashSet<string>();

        foreach (var res in resolutions)
        {
            string option = $"{res.width}x{res.height}";
            if (!addedResolutions.Contains(option))
            {
                filteredResolutions.Add(res);
                addedResolutions.Add(option);
            }
        }

        // Populate dropdown
        List<string> options = new List<string>();
        int currentIndex = 0;
        
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = $"{filteredResolutions[i].width}x{filteredResolutions[i].height}";
            options.Add(option);

            // Select the current screen resolution
            if (filteredResolutions[i].width == Screen.width &&
                filteredResolutions[i].height == Screen.height)
            {
                currentIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);

        // Load saved resolution index
        int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.width);
        int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.height);
        int savedIndex = currentIndex; // Default to current resolution index

        // Find the saved resolution index
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            if (filteredResolutions[i].width == savedWidth && 
                filteredResolutions[i].height == savedHeight)
            {
                savedIndex = i;
                break;
            }
        }

        resolutionDropdown.value = savedIndex;
        resolutionDropdown.RefreshShownValue();

        SetResolution(savedIndex);
    }
    public void SetResolution(int index)
    {
        if (index < 0 || index >= filteredResolutions.Count) return;

        Resolution res = filteredResolutions[index];

        FullScreenMode mode = fullscreenToggle.isOn
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;
        Screen.SetResolution(res.width, res.height, mode);

        PlayerPrefs.SetInt(ResolutionWidthKey, res.width);
        PlayerPrefs.SetInt(ResolutionHeightKey, res.height);
        PlayerPrefs.Save();
    }
    #endregion
    #region Load/Save Settings
    void LoadSettings()
    {
        float music = PlayerPrefs.GetFloat(MusicKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SFXKey, 1f);
        int fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1);

        musicSlider.value = music;
        sfxSlider.value = sfx;
        fullscreenToggle.isOn = fullscreen == 1;

        SetVolumeMusic(music);
        SetVolumeSFX(sfx);
        SetFullScreen(fullscreen == 1);
    }
    #endregion
}
