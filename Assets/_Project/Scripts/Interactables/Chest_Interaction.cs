using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private Animator chestAnimator;

    private bool isPlayerInRange = false; // рядом ли игрок
    public bool isOpen = false; // открыт ли сундук

    [Header("UI")]
    public GameObject inventoryUI; 

    void Start()
    {
        chestAnimator = GetComponent<Animator>();

        if (chestAnimator == null)
        {
            Debug.LogError("На объекте сундука не найден компонент Animator!");
        }

        // Скрываем UI при старте
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }
        else
        {
            Debug.LogError("inventoryUI не назначен в Inspector!");
        }
    }

    void Update()
    {
        // Нажатие E рядом с сундуком
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
                OpenChest();
            else
                CloseChest();
        }
    }

    private void OpenChest()
    {
        isOpen = true;

        // Анимация
        if (chestAnimator != null)
        {
            chestAnimator.SetBool("IsOpen", true);
        }

        // ВКЛЮЧАЕМ UI
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
        }

        Debug.Log("Сундук открыт");
    }

    private void CloseChest()
    {
        isOpen = false;

        // Анимация
        if (chestAnimator != null)
        {
            chestAnimator.SetBool("IsOpen", false);
        }

        // ВЫКЛЮЧАЕМ UI
        if (inventoryUI != null)
        {
            inventoryUI.SetActive(false);
        }

        Debug.Log("Сундук закрыт");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Нажми E, чтобы открыть сундук");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            // Авто-закрытие при уходе
            if (isOpen)
                CloseChest();
        }
    }
}