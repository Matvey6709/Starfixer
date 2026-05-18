using UnityEngine;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    [Header("Данные для инвентаря")]
    public Item item; // Сюда настраиваешь, что это за предмет (ID, имя, количество)

    [Header("Уникальный паспорт объекта")]
    [Tooltip("Придумай уникальный ID для этого конкретного объекта на сцене (например: dump_patch_1)")]
    public string uniqueID;

    private void Start()
    {
        // Проверяем, существует ли база данных
        if (DataManager.Instance == null || DataManager.Instance.gameData == null) return;

        // Если забыл написать ID в инспекторе — скрипт предупредит
        if (string.IsNullOrEmpty(uniqueID))
        {
            Debug.LogWarning($"У объекта {gameObject.name} не заполнен uniqueID! Он будет возрождаться.");
            return;
        }

        // ПРОВЕРКА: Если этот паспорт уже есть в списке собранных — уничтожаем предмет сразу при загрузке
        if (DataManager.Instance.gameData.collectedItems.Contains(uniqueID))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (DataManager.Instance == null || DataManager.Instance.gameData == null) return;

            // 1. Добавляем предмет в глобальный инвентарь DataManager
            AddItemToGlobalInventory();
            SoundManager.PlayPickup();

            // 2. Записываем паспорт объекта в список собранных вещей
            if (!string.IsNullOrEmpty(uniqueID) && !DataManager.Instance.gameData.collectedItems.Contains(uniqueID))
            {
                DataManager.Instance.gameData.collectedItems.Add(uniqueID);
                Debug.Log($"Объект {uniqueID} добавлен в список собранных навсегда.");
            }

            // И удаляем его со сцены
            Destroy(gameObject);
        }
    }

    private void AddItemToGlobalInventory()
    {
        List<Item> playerInv = DataManager.Instance.gameData.inventory;
        bool found = false;

        foreach (var invItem in playerInv)
        {
            if (invItem.id == item.id)
            {
                invItem.amount += item.amount;
                found = true;
                break;
            }
        }

        if (!found)
        {
            playerInv.Add(new Item { id = item.id, itemName = item.itemName, amount = item.amount });
        }
    }
}