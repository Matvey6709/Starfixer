using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [SerializeField] private float skipInputDelay = 0.5f;

    private VideoPlayer videoPlayer;
    private float startTime;
    private bool finished;

    void Start()
    {
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
        if (finished) return;
        if (Time.unscaledTime - startTime < skipInputDelay) return;
        if (Input.anyKeyDown) LoadLobby();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadLobby();
    }

    void LoadLobby()
    {
        if (finished) return;
        finished = true;
        videoPlayer.loopPointReached -= OnVideoFinished;
        videoPlayer.prepareCompleted -= OnPrepared;
        SceneManager.LoadScene("Lobby");
    }
}
