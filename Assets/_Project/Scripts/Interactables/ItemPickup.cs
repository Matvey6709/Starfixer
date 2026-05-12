using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

private void OnTriggerEnter2D(Collider2D other)
{
    if (other.CompareTag("Player"))
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player == null)
        {
            Debug.LogError("Ошибка: На игроке не найден скрипт PlayerController!");
            return;
        }
        if (player.inventory == null)
        {
            Debug.LogError("Ошибка: В PlayerController переменная inventory не инициализирована (null)!");
            return;
        }
        if (item == null)
        {
            Debug.LogError("Ошибка: В объекте " + gameObject.name + " не заполнены данные в поле Item!");
            return;
        }

        player.inventory.AddItem(item);
        Debug.Log("Подобран: " + item.itemName);
        Destroy(gameObject);
    }
}
}