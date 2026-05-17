using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private List<GameObject> pages = new List<GameObject>();
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text pageIndicator;

    private int currentPage;

    private bool wired;

    private void Awake()
    {
        WireListeners();
    }

    private void WireListeners()
    {
        if (wired) return;
        if (prevButton != null) prevButton.onClick.AddListener(ShowPrev);
        if (nextButton != null) nextButton.onClick.AddListener(ShowNext);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        wired = true;
    }

    public void Open()
    {
        if (panel != null) panel.SetActive(true);
        WireListeners();
        currentPage = 0;
        Refresh();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
    }

    private void ShowPrev()
    {
        if (currentPage > 0)
        {
            currentPage--;
            Refresh();
        }
    }

    private void ShowNext()
    {
        if (currentPage < pages.Count - 1)
        {
            currentPage++;
            Refresh();
        }
    }

    private void Refresh()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            if (pages[i] != null) pages[i].SetActive(i == currentPage);
        }

        if (pageIndicator != null)
        {
            pageIndicator.text = $"{currentPage + 1}/{pages.Count}";
        }

        if (prevButton != null) prevButton.interactable = currentPage > 0;
        if (nextButton != null) nextButton.interactable = currentPage < pages.Count - 1;
    }
}
