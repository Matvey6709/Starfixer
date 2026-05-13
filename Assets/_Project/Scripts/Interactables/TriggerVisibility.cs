using UnityEngine;

public class TriggerVisibility : MonoBehaviour
{
    public SpriteRenderer targetRenderer; // Сюда перетащи микросхему

    void Start()
    {
        // В начале игры делаем микросхему невидимой
        if (targetRenderer != null)
            targetRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetRenderer.enabled = true; // Показываем
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            targetRenderer.enabled = false; // Снова прячем, если игрок ушел
        }
    }
}