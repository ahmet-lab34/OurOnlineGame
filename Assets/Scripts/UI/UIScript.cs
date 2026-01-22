using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using System.Collections.Generic;

public class UIScript : MonoBehaviour
{
    //public VideoPlayer videoPlayer;
    [SerializeField] private TMP_Text Coinss;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private GameObject ESC_Menu;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private Button ESC_Options;
    [SerializeField] private Button ESC_Exit;
    [SerializeField] private Button ESC_ContinueButton;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private TMP_Dropdown resolutionDropDown;
    
    private Resolution[] resolutions;
    private List<Resolution> filteredResolution;

    private float currentRefreshRate;
    private int currentResolutionIndex;

    [Obsolete]

    void Start()
    {
        //videoPlayer.loopPointReached += OnVideoEnd;

        QuitButton.onClick.AddListener(() => OnButtonPressed(QuitButton));
        ESC_ContinueButton.onClick.AddListener(() => OnButtonPressed(ESC_ContinueButton));
        ESC_Options.onClick.AddListener(() => OnButtonPressed(ESC_Options));
        ESC_Exit.onClick.AddListener(() => OnButtonPressed(ESC_Exit));
        Coinss.text = CoinCount.coins.ToString(); 

        resolutions = Screen.resolutions;
        filteredResolution = new List<Resolution>();
        resolutionDropDown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolution.Add(resolutions[i]);
            }
        }
        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolution.Count; i++)
        {
            string resolutionOption = filteredResolution[i].width + "x" + filteredResolution[i].height + " " + filteredResolution[i].refreshRate + "HZ";
            options.Add(resolutionOption);
            if (filteredResolution[i].width == Screen.width && filteredResolution[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.value = currentResolutionIndex;
        resolutionDropDown.RefreshShownValue();
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolution[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
    void OnVideoEnd(VideoPlayer vp)
    {
        //videoPlayer.gameObject.SetActive(false);
        //MainMenu.SetActive(true);
    }
    public void RestartScene()
    {
        PlayerPrefs.SetInt("SkipMainMenu", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void UIDie()
    {
        deathPanel.SetActive(true);
        PlayerObject.SetActive(false);
        Time.timeScale = 0f;
    }
    void OnButtonPressed(Button button)
    {
        if (button == RestartButton)
        {
            Debug.Log("Just the Restart Button Is Pressed !");
            GravitySwitcherUpsideDown.isRotated = false;
            RestartScene();
        }
        if (button == ESC_ContinueButton)
        {
            ESC_Menu.SetActive(false);
            Time.timeScale = 1f;
        }
        if (button == QuitButton)
        {
            Application.Quit();
        }
    }
}

