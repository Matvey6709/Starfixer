using UnityEngine;

public class EnergyGenerator : MonoBehaviour
{
    [Header("Визуальные состояния (0%, 25%, 50%, 75%, 100%)")]
    public SpriteRenderer spriteRenderer;
    public Sprite[] generatorSprites; 

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
        if (isFullyCharged || !playerInTrigger) return;

        if (Input.GetKey(KeyCode.Space))
        {
            chargeTimer += Time.deltaTime;
            CheckProgress();
        }
    }

    void CheckProgress()
    {
        bool phaseChanged = false;

        if (currentPhase == 0 && chargeTimer >= 2.5f)
        {
            currentPhase = 1;
            phaseChanged = true;
        }
        else if (currentPhase == 1 && chargeTimer >= 5.0f)
        {
            currentPhase = 2;
            phaseChanged = true;
        }
        else if (currentPhase == 2 && chargeTimer >= 7.5f)
        {
            currentPhase = 3;
            phaseChanged = true;
        }
        else if (currentPhase == 3 && chargeTimer >= 10.0f)
        {
            currentPhase = 4;
            isFullyCharged = true;
            phaseChanged = true;
        }

        if (phaseChanged)
        {
            UpdateSprite();

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
        if (collision.CompareTag("Player") && !isFullyCharged)
        {
            playerInTrigger = true;
            InteractionHintUI.Show("Удерживай пробел, что выключить генератор.");
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
            
            if (CameraGlowController.Instance != null)
            {
                CameraGlowController.Instance.SetGlow(false);
            }
        }
    }

    public void ResetAndDisable()
    {
        currentPhase = 0;        
        chargeTimer = 0f;       
        isFullyCharged = false;  
        playerInTrigger = false; 

        UpdateSprite();      

        Collider2D generatorCollider = GetComponent<Collider2D>();
        if (generatorCollider != null)
        {
            generatorCollider.enabled = false;
            Debug.Log($"[ГЕНЕРАТОР] {gameObject.name} успешно сброшен и отключен.");
        }

        if (CameraGlowController.Instance != null)
        {
            CameraGlowController.Instance.SetGlow(false);
        }
    }
}