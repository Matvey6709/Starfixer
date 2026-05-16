using UnityEngine;

public class EnergyGenerator : MonoBehaviour
{
    [Header("Визуальные состояния (0%, 25%, 50%, 75%, 100%)")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] generatorSprites; // Сюда в инспекторе перетащи 5 спрайтов по порядку

    [Header("Настройки времени зажима")]
    private float chargeTimer = 0f;
    private int currentPhase = 0; // 0 = 0%, 1 = 25%, 2 = 50%, 3 = 75%, 4 = 100%
    private bool playerInTrigger = false;
    private bool isFullyCharged = false;

    public bool IsFullyCharged => isFullyCharged;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void Update()
    {
        // Если уже заряжен на 100% или игрок не в триггере — ничего не делаем
        if (isFullyCharged || !playerInTrigger) return;

        // Если игрок зажал Пробел, прогресс плавно накапливается
        if (Input.GetKey(KeyCode.Space))
        {
            chargeTimer += Time.deltaTime;
            CheckProgress();
        }
        // Блок сброса Input.GetKeyUp удален, чтобы время просто замирало при отпускании клавиши
    }

    void CheckProgress()
    {
        bool phaseChanged = false;

        // Фаза 1: 25% на 2.5 секундах
        if (currentPhase == 0 && chargeTimer >= 2.5f)
        {
            currentPhase = 1;
            phaseChanged = true;
        }
        // Фаза 2: 50% на 5.0 секундах
        else if (currentPhase == 1 && chargeTimer >= 5.0f)
        {
            currentPhase = 2;
            phaseChanged = true;
        }
        // Фаза 3: 75% на 7.5 секундах
        else if (currentPhase == 2 && chargeTimer >= 7.5f)
        {
            currentPhase = 3;
            phaseChanged = true;
        }
        // Фаза 4: 100% на 10.0 секундах
        else if (currentPhase == 3 && chargeTimer >= 10.0f)
        {
            currentPhase = 4;
            isFullyCharged = true;
            phaseChanged = true;
        }

        if (phaseChanged)
        {
            UpdateSprite();

            // Оповещаем менеджер при любом изменении фазы (для точного подсчета процентов)
            if (BossFightManager.Instance != null)
            {
                BossFightManager.Instance.CheckGeneratorsProgress();
            }
        }
    }

    void UpdateSprite()
    {
        if (generatorSprites != null && currentPhase < generatorSprites.Length)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = generatorSprites[currentPhase];
            }
        }
    }

    public int GetCurrentPhase()
    {
        return currentPhase;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Если генератор УЖЕ полностью заряжен, свечение включать не нужно
        if (collision.CompareTag("Player") && !isFullyCharged)
        {
            playerInTrigger = true;

            // Включаем синее свечение на камере
            if (CameraGlowController.Instance != null)
            {
                CameraGlowController.Instance.SetGlow(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;

            // Выключаем синее свечение на камере
            if (CameraGlowController.Instance != null)
            {
                CameraGlowController.Instance.SetGlow(false);
            }
        }
    }
}