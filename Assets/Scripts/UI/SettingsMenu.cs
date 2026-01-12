using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GroundCHK GroundCheck;
    public AudioMixer audioMixer;
    public AudioSource BackgroundAudio;
    public TMP_Dropdown resolutionDropDown;
    Resolution[] resolutions;
    void Start()
    {
        float val;
        bool exists = audioMixer.GetFloat("MusicVolume", out val);
        Debug.Log("MusicVolume exists: " + exists);

        
        resolutions = Screen.resolutions;

        resolutionDropDown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }

    public void SetVolumeMusic(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }
    public void SetVolumeSFX(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0001f, 1f);
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SetFullScreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}

