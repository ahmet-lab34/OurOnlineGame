using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private GroundCHK GroundCheck;
    public AudioClip OptionsSFX;
    public AudioSource OptionsSFXS;
    public AudioMixer audioMixer;
    public AudioSource BackgroundAudio;
    
    public void optionsSFXMethod()
    {
        OptionsSFXS.PlayOneShot(OptionsSFX);
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

