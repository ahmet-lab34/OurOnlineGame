using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIScript : MonoBehaviour
{
    [SerializeField] private TMP_Text Coinss;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private GameObject DeathPanel;
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject GameUI;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject GameMap1;
    [SerializeField] private GameObject PleaseDoNotQuit;
    [SerializeField] private GameObject ESC_Menu;
    [SerializeField] private Button ESC_Options;
    [SerializeField] private Button ESC_Exit;
    [SerializeField] private Button RestartButton;
    [SerializeField] private Button MainMenuButton;

    [SerializeField] private Button MainPlayButton;
    [SerializeField] private Button MainEndlessButton;
    [SerializeField] private Button MainOptionsButton;
    [SerializeField] private Button MainExitButton;
    [SerializeField] private Button ESC_ContinueButton;
    [SerializeField] private Button StayButton;
    [SerializeField] private Button QuitButton;
    [SerializeField] private Button OptionsBackButton;

    void Start()
    {
        if (PlayerPrefs.GetInt("SkipMainMenu", 0) == 1)
        {
            MainMenu.SetActive(false);
            GameMap1.SetActive(true);
            GameUI.SetActive(true);
            Time.timeScale = 1f;
            PlayerPrefs.SetInt("SkipMainMenu", 0);
        }
        else
        {
            MainMenu.SetActive(true);
        }
        RestartButton.onClick.AddListener(() => OnButtonPressed(RestartButton));
        MainMenuButton.onClick.AddListener(() => OnButtonPressed(MainMenuButton));

        QuitButton.onClick.AddListener(() => OnButtonPressed(QuitButton));
        StayButton.onClick.AddListener(() => OnButtonPressed(StayButton));
        MainPlayButton.onClick.AddListener(() => OnButtonPressed(MainPlayButton));
        MainEndlessButton.onClick.AddListener(() => OnButtonPressed(MainEndlessButton));
        MainOptionsButton.onClick.AddListener(() => OnButtonPressed(MainOptionsButton));
        OptionsBackButton.onClick.AddListener(() => OnButtonPressed(OptionsBackButton));
        MainExitButton.onClick.AddListener(() => OnButtonPressed(MainExitButton));
        ESC_ContinueButton.onClick.AddListener(() => OnButtonPressed(ESC_ContinueButton));
        ESC_Options.onClick.AddListener(() => OnButtonPressed(ESC_Options));
        ESC_Exit.onClick.AddListener(() => OnButtonPressed(ESC_Exit));
        Coinss.text = CoinCount.coins.ToString(); 
    }
    public void RestartScene()
    {
        PlayerPrefs.SetInt("SkipMainMenu", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void UIDie()
    {
        DeathPanel.SetActive(true);
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
        if (button == MainMenuButton)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }

        if (button == ESC_ContinueButton)
        {
            ESC_Menu.SetActive(false);
            Time.timeScale = 1f;
        }
        if (button == MainPlayButton)
        {
            GameMap1.SetActive(true);
            GameUI.SetActive(true);
            MainMenu.SetActive(false);
        }
        if (button == MainEndlessButton)
        {

        }
        if (button == ESC_Options)
        {
            Time.timeScale = 1f;
            ESC_Menu.SetActive(false);
            GameMap1.SetActive(false);
            Settings.SetActive(true);
        }
        if (button == MainOptionsButton)
        {
            Settings.SetActive(true);
            MainMenu.SetActive(false);
        }
        if (button == OptionsBackButton)
        {
            MainMenu.SetActive(true);
            Settings.SetActive(false);
        }
        if (button == ESC_Exit)
        {
            Application.Quit();
        }
        if (button == MainExitButton)
        {
            MainMenu.SetActive(false);
            PleaseDoNotQuit.SetActive(true);
        }
        if (button == StayButton)
        {
            Scene currentScene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(currentScene.name);
        }
        if (button == QuitButton)
        {
            Application.Quit();
        }
    }
}
