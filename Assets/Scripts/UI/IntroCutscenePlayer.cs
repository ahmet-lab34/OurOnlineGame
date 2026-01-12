using System;
using UnityEngine;
using UnityEngine.Video;

public class IntroCutscenePlayer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject introPanel;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Settings")]
    [SerializeField] private bool playOnlyOnce = false;
    [SerializeField] private string playedKey = "IntroPlayed";

    private Action onFinished;
    private bool isPlaying;
    private float previousTimeScale = 1f;

    private void Awake()
    {
        if (introPanel != null)
            introPanel.SetActive(false);

        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.skipOnDrop = false;
            videoPlayer.waitForFirstFrame = true;

            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    public void TryPlayIntroOrStartGame(Action startGameCallback)
    {
        if (introPanel == null || videoPlayer == null)
        {
            startGameCallback?.Invoke();
            return;
        }

        if (playOnlyOnce && PlayerPrefs.GetInt(playedKey, 0) == 1)
        {
            startGameCallback?.Invoke();
            return;
        }

        onFinished = startGameCallback;

        introPanel.SetActive(true);
        isPlaying = true;

        previousTimeScale = Time.timeScale;
        Time.timeScale = 1f;

        videoPlayer.Stop();
        videoPlayer.time = 0;
        videoPlayer.frame = 0;

        videoPlayer.prepareCompleted -= OnPrepared;
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
    }

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (videoPlayer.isPlaying)
                videoPlayer.Stop();

            Finish();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        Finish();
    }

    private void Finish()
    {
        if (!isPlaying) return;
        isPlaying = false;

        if (playOnlyOnce)
        {
            PlayerPrefs.SetInt(playedKey, 1);
            PlayerPrefs.Save();
        }

        if (introPanel != null)
            introPanel.SetActive(false);

        Time.timeScale = previousTimeScale;

        onFinished?.Invoke();
        onFinished = null;
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
