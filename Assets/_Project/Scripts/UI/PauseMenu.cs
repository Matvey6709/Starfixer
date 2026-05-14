using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button lobbyButton;

    private void Start()
    {
        pausePanel.SetActive(false);
        menuButton.onClick.AddListener(OpenPause);
        continueButton.onClick.AddListener(ClosePause);
        lobbyButton.onClick.AddListener(GoToLobby);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void TogglePause()
    {
        if (pausePanel.activeSelf)
        {
            ClosePause();
        }
        else
        {
            OpenPause();
        }
    }
    private void OpenPause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void ClosePause()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void GoToLobby()
    {
        Time.timeScale = 1f;
        if (DataManager.Instance != null)
        {
            DataManager.Instance.SaveData();
            DataManager.Instance.RestoreFromCheckpoint();
        }
        SceneManager.LoadScene("Lobby");
    }
}
