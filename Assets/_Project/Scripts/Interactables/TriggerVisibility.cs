using UnityEngine;

public class TriggerVisibility : MonoBehaviour
{
    public SpriteRenderer targetRenderer; 

    void Start()
    {
        if (targetRenderer != null)
            targetRenderer.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetRenderer != null)
        {
            targetRenderer.enabled = true; 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (targetRenderer == null) return;

        if (other.CompareTag("Player"))
        {
            targetRenderer.enabled = false; 
        }
    }
}