using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private TutorialUI tutorialUI;
    [SerializeField] private string introSceneName = "IntroScene";

    private void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        if (tutorialButton != null) tutorialButton.onClick.AddListener(OpenTutorial);
    }

    private void OnContinueClicked()
    {
        var data = DataManager.Instance != null ? DataManager.Instance.gameData : null;
        if (data != null && !data.introWatched)
        {
            SceneManager.LoadScene(introSceneName);
            return;
        }

        GameManager.Instance.ContinueGame();
    }

    private void OpenTutorial()
    {
        if (tutorialUI != null) tutorialUI.Open();
    }
}
