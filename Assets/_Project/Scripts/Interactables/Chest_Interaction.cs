using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private Animator chestAnimator;

    private bool isPlayerInRange = false; 
    public bool isOpen = false; 

    [Header("UI")]
    public GameObject inventoryUI; 

    void Start()
    {
        chestAnimator = GetComponent<Animator>();

        if (chestAnimator == null)
        {
            Debug.LogError("На объекте сундука не найден компонент Animator!");
        }

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

        if (chestAnimator != null)
        {
            chestAnimator.SetBool("IsOpen", true);
        }

        if (inventoryUI != null)
        {
            inventoryUI.SetActive(true);
        }

        Debug.Log("Сундук открыт");
        SoundManager.PlayChestOpen();
    }

    private void CloseChest()
    {
        isOpen = false;

        if (chestAnimator != null)
        {
            chestAnimator.SetBool("IsOpen", false);
        }

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
            InteractionHintUI.Show("Нажми E — открыть сундук");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            if (isOpen)
                CloseChest();
        }
    }
}