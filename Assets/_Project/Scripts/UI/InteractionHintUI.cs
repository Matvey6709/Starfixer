using System.Collections;
using TMPro;
using UnityEngine;

public class InteractionHintUI : MonoBehaviour
{
    public static InteractionHintUI Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text hintText;
    [SerializeField] private float showDuration = 2f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (panel != null) panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public static void Show(string text)
    {
        if (Instance != null) Instance.ShowInternal(text);
    }

    private void ShowInternal(string text)
    {
        if (panel == null || hintText == null) return;
        hintText.text = text;
        panel.SetActive(true);
        if (hideRoutine != null) StopCoroutine(hideRoutine);
        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        if (panel != null) panel.SetActive(false);
        hideRoutine = null;
    }
}
