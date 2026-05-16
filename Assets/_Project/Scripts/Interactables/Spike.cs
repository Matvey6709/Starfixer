using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour
{
    [Header("Timing Settings")]
    public float loweredDuration = 0.5f; // Время в лунках
    public float raisedDuration = 1.0f;  // Время в поднятом состоянии

    [Header("Damage & Knockback")]
    public float damageAmount = 10f;
    public float knockbackForce = 12f;
    public float knockbackDuration = 0.15f;
    public float knockbackCooldown = 0.5f;

    [Header("Visual Settings (Смена спрайтов)")]
    public SpriteRenderer spriteRenderer;
    public Sprite loweredSprite; // Перетащи сюда спрайт ПУСТЫХ ДЫР
    public Sprite raisedSprite;  // Перетащи сюда спрайт ОТКРЫТЫХ ШИПОВ

    private bool isRaised = false;
    private float nextDamageTime;

    void Start()
    {
        // Если забыл привязать в инспекторе, ищем на этом же объекте
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Начинаем цикл ловушки
        StartCoroutine(SpikesCycleRoutine());
    }

    IEnumerator SpikesCycleRoutine()
    {
        while (true)
        {
            // --- ШИПЫ ОПУЩЕНЫ ---
            isRaised = false;
            if (spriteRenderer != null && loweredSprite != null)
            {
                spriteRenderer.sprite = loweredSprite;
            }
            yield return new WaitForSeconds(loweredDuration);

            // --- ШИПЫ ПОДНЯТЫ ---
            isRaised = true;
            if (spriteRenderer != null && raisedSprite != null)
            {
                spriteRenderer.sprite = raisedSprite;
            }
            yield return new WaitForSeconds(raisedDuration);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRaised) TryHitPlayer(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isRaised) TryHitPlayer(collision);
    }

    private void TryHitPlayer(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= nextDamageTime)
        {
            PlayerOxygen oxygen = collision.GetComponentInParent<PlayerOxygen>();
            PlayerController controller = collision.GetComponentInParent<PlayerController>();

            if (oxygen != null && controller != null)
            {
                oxygen.TakeDamage(damageAmount);

                Vector2 knockbackDir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;
                if (knockbackDir == Vector2.zero) knockbackDir = Vector2.up;

                controller.ApplyKnockback(knockbackDir, knockbackForce, knockbackDuration);
                nextDamageTime = Time.time + knockbackCooldown;
            }
        }
    }
}