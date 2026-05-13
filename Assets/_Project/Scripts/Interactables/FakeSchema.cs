using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    [Header("Настройки спрайтов")]
    public Sprite activeSprite;  // Спрайт, который будет ПРИ входе
    public Sprite idleSprite;    // Спрайт, который вернется ПОСЛЕ выхода

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (idleSprite != null) sr.sprite = idleSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.sprite = activeSprite;
            Debug.Log("Спрайт изменен на активный");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            sr.sprite = idleSprite;
            Debug.Log("Спрайт возвращен в исходное состояние");
        }
    }
}