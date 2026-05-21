using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [SerializeField] private float skipInputDelay = 0.5f;
    [SerializeField] private string nextSceneName = "SpaceShip";
    [SerializeField] private string fallbackSceneName = "Lobby";

    private VideoPlayer videoPlayer;
    private float startTime;
    private bool finished;
    private bool skippingBecauseAlreadyWatched;

    void Start()
    {
        if (DataManager.Instance != null && DataManager.Instance.gameData != null
            && DataManager.Instance.gameData.introWatched)
        {
            skippingBecauseAlreadyWatched = true;
            SceneManager.LoadScene(fallbackSceneName);
            return;
        }

        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
        startTime = Time.unscaledTime;
    }

    void OnPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    void Update()
    {
        if (finished || skippingBecauseAlreadyWatched) return;
        if (Time.unscaledTime - startTime < skipInputDelay) return;
        if (Input.anyKeyDown) LoadNextScene();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    void LoadNextScene()
    {
        if (finished) return;
        finished = true;

        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
            videoPlayer.prepareCompleted -= OnPrepared;
        }

        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData.introWatched = true;
            DataManager.Instance.SaveData();
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
