using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        // 1. Ищем контроллер (лучше искать и в родителях на всякий случай)
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player == null)
        {
            Debug.LogError("Ошибка: На игроке не найден скрипт PlayerController!");
            return;
        }

        // 2. Проверяем, существует ли инвентарь в контроллере
        if (player.inventory == null)
        {
            Debug.LogError("Ошибка: В PlayerController переменная inventory не инициализирована (null)!");
            return;
        }

        // 3. Проверяем, назначен ли предмет в инспекторе кубика
        if (item == null)
        {
            Debug.LogError("Ошибка: В объекте " + gameObject.name + " не заполнены данные в поле Item!");
            return;
        }

        // Если всё ок — добавляем
        player.inventory.AddItem(item);
        Debug.Log("Подобран: " + item.itemName);
        Destroy(gameObject);
    }
}
}