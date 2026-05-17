using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AudioToggleButton : MonoBehaviour
{
    public enum Channel { Music, Sfx }

    [SerializeField] private Channel channel;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private Image background;
    [SerializeField] private Color onColor = new Color(0.2f, 0.55f, 0.3f, 1f);
    [SerializeField] private Color offColor = new Color(0.55f, 0.2f, 0.2f, 1f);

    private void Start()
    {
        if (button != null) button.onClick.AddListener(Toggle);
        UpdateVisual();
    }

    private void Toggle()
    {
        if (channel == Channel.Music)
            SoundManager.SetMusicMuted(!SoundManager.IsMusicMuted);
        else
            SoundManager.SetSfxMuted(!SoundManager.IsSfxMuted);
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        bool muted = channel == Channel.Music ? SoundManager.IsMusicMuted : SoundManager.IsSfxMuted;
        if (stateText != null) stateText.text = muted ? "ВЫКЛ" : "ВКЛ";
        if (background != null) background.color = muted ? offColor : onColor;
    }
}
