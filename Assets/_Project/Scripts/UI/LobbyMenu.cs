using UnityEngine;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button tutorialButton;
    [SerializeField] private TutorialUI tutorialUI;

    private void Start()
    {
        continueButton.onClick.AddListener(OnContinueClicked);
        if (tutorialButton != null) tutorialButton.onClick.AddListener(OpenTutorial);
    }

    private void OnContinueClicked()
    {
        GameManager.Instance.ContinueGame();
    }

    private void OpenTutorial()
    {
        if (tutorialUI != null) tutorialUI.Open();
    }
}
