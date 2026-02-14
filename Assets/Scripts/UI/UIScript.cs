using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Netcode;

public class UIScript : MonoBehaviour
{
    [Header("Player & Panels")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private GameObject escMenu;
    [SerializeField] private GameObject deathPanel;

    [Header("Buttons")]
    [SerializeField] private Button escOptionsButton;
    [SerializeField] private Button escExitButton;
    [SerializeField] private Button escContinueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text refreshRateText;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private float currentRefreshRate;
    private int currentResolutionIndex;

    [System.Obsolete]
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // Button listeners
        quitButton.onClick.AddListener(() => OnButtonPressed(quitButton));
        escContinueButton.onClick.AddListener(() => OnButtonPressed(escContinueButton));
        escOptionsButton.onClick.AddListener(() => OnButtonPressed(escOptionsButton));
        escExitButton.onClick.AddListener(() => OnButtonPressed(escExitButton));
        restartButton.onClick.AddListener(() => OnButtonPressed(restartButton));

        // Update refresh rate display
        if (refreshRateText != null)
            refreshRateText.text = $"{Screen.currentResolution.refreshRate}Hz";

        // Resolution setup
        SetupResolutions();
    }

    #region Resolution

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
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        if (index < 0 || index >= filteredResolutions.Count) return;

        Resolution res = filteredResolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    #endregion

    #region Networking

    public void StartHost() => NetworkManager.Singleton.StartHost();
    public void StartClient() => NetworkManager.Singleton.StartClient();
    public void StartServer() => NetworkManager.Singleton.StartServer();

    #endregion

    #region Scene & Game Control

    private void OnButtonPressed(Button button)
    {
        if (button == restartButton)
        {
            ResetPlayerGravity();
            RestartScene();
        }
        else if (button == escContinueButton)
        {
            escMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else if (button == quitButton)
        {
            Application.Quit();
        }
    }

    private void RestartScene()
    {
        PlayerPrefs.SetInt("SkipMainMenu", 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UIDie()
    {
        deathPanel.SetActive(true);
        if (playerObject != null) playerObject.SetActive(false);
        Time.timeScale = 0f;
    }

    #endregion

    #region Gravity

    /// <summary>
    /// Reset global 2D gravity to default downward if it was flipped
    /// </summary>
    private void ResetPlayerGravity()
    {
        if (playerObject == null) return;

        Rigidbody2D rb = playerObject.GetComponent<Rigidbody2D>();
        PlayerStats playerStats = playerObject.GetComponent<PlayerStats>();
        if (rb != null && playerStats != null)
        {
            // Restore player's gravity scale
            rb.gravityScale = playerStats.GetDefaultGravityScale();

            // Make player upright
            playerStats.SetUpsideDownState(false);
        }
    }


    #endregion

    #region Fullscreen

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }

    #endregion
}
