using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private Animator chestAnimator;
    private bool isPlayerInRange = false; // Состояние: рядом ли игрок
    private bool isOpen = false; // Состояние: открыт ли уже сундук

    void Start()
    {
        chestAnimator = GetComponent<Animator>();
        if (chestAnimator == null)
        {
            Debug.LogError("На объекте сундука не найден компонент Animator!");
        }
    }

    // ТРИГГЕРЫ: просто фиксируют присутствие игрока
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Убедитесь, что у вашего объекта Player стоит тег "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("Вы рядом с сундуком. Нажмите E для открытия.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("Вы отошли от сундука.");
        }
    }

    // ЛОГИКА ВЗАИМОДЕЙСТВИЯ (Ловим нажатие 'E')
    void Update()
    {
        // Проверяем нажатие E каждый кадр, только если игрок рядом и сундук еще не открыт
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
                OpenChest();
            else CloseChest();
        }
    }

    private void OpenChest()
    {
        if (chestAnimator != null)
        {
            isOpen = true; // Теперь он считается открытым
            chestAnimator.SetBool("IsOpen", true); // Запускаем анимацию в Аниматоре
            Debug.Log("Сундук склада открыт!");

            // СЮДА мы в будущем добавим логику самого склада:
            // Например, вызов окна инвентаря склада или передачу предметов Эрику
        }
    }

    private void CloseChest()
    {
        if (chestAnimator != null)
        {
            isOpen = false; // Теперь он считается закрытым
            chestAnimator.SetBool("IsOpen", false); // Запускаем анимацию в Аниматоре
            Debug.Log("Сундук склада закрыт!");
        }
    }
}