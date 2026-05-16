using UnityEngine;

public class ActivationLever : MonoBehaviour
{
    [Header("Компоненты")]
    public SpriteRenderer spriteRenderer;

    [Header("Спрайты состояний рычага")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;
    public Sprite pulledSprite;

    [Header("Настройки выпадения предмета")]
    [Tooltip("Перетащи сюда префаб предмета, который должен выпасть")]
    public GameObject itemPrefab;

    [Tooltip("Точка, из которой вылетит предмет (если не назначено, вылетит из центра рычага)")]
    public Transform itemSpawnPoint;

    [Header("Физика выталкивания предмета")]
    public bool applyForce = true; // Нужно ли подтолкнуть предмет при появлении
    public Vector2 throwForce = new Vector2(2f, 3f); // Направление и сила прыжка 

    private bool isActivatedByGenerators = false;
    private bool isPulled = false;
    private bool playerInTrigger = false;

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (inactiveSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError($"[РЫЧАГ] На объекте {gameObject.name} НЕТ КОЛЛАЙДЕРА!");
        }
    }

    void Update()
    {
        if (!isActivatedByGenerators || isPulled || !playerInTrigger) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Pull();
        }
    }

    [ContextMenu("Test Activate Lever")]
    public void EnableLever()
    {
        isActivatedByGenerators = true;

        if (spriteRenderer != null && activeSprite != null)
        {
            spriteRenderer.sprite = activeSprite;
            Debug.Log("[РЫЧАГ] Успешно активирован! Спрайт сменился на АКТИВНЫЙ.");
        }
    }

    void Pull()
    {
        isPulled = true;

        if (spriteRenderer != null && pulledSprite != null)
        {
            spriteRenderer.sprite = pulledSprite;
            Debug.Log("[РЫЧАГ] Игрок опустил рычаг!");
        }

        // СПАВН ПРЕДМЕТА
        SpawnDroppedItem();

        if (BossFightManager.Instance != null)
        {
            BossFightManager.Instance.TriggerBossDeath();
        }
    }

    void SpawnDroppedItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogError("[РЫЧАГ] Нет префаба предмета в поле Item Prefab! Выпадать нечему.");
            return;
        }

        // Определяем точку спавна: либо специальный Transform, либо позиция самого рычага
        Vector3 spawnPosition = itemSpawnPoint != null ? itemSpawnPoint.position : transform.position;

        // Создаем предмет на сцене
        GameObject droppedItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"[РЫЧАГ] Предмет {droppedItem.name} успешно выброшен!");

        // Если включена сила выталкивания, пробуем подбросить предмет
        if (applyForce)
        {
            Rigidbody2D rb = droppedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Сбрасываем текущую скорость, если она была, и добавляем импульс
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(throwForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogWarning("[РЫЧАГ] На вылетевшем предмете нет компонента Rigidbody2D, физический импульс прыжка не применился.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }
}