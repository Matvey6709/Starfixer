using UnityEngine;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    [Header("Данные для инвентаря")]
    public Item item; 

    [Header("Уникальный паспорт объекта")]
    [Tooltip("Придумай уникальный ID для этого конкретного объекта на сцене (например: dump_patch_1)")]
    public string uniqueID;

    private void Start()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null) return;

        if (string.IsNullOrEmpty(uniqueID))
        {
            Debug.LogWarning($"У объекта {gameObject.name} не заполнен uniqueID! Он будет возрождаться.");
            return;
        }

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

            AddItemToGlobalInventory();
            SoundManager.PlayPickup();

            if (!string.IsNullOrEmpty(uniqueID) && !DataManager.Instance.gameData.collectedItems.Contains(uniqueID))
            {
                DataManager.Instance.gameData.collectedItems.Add(uniqueID);
                Debug.Log($"Объект {uniqueID} добавлен в список собранных навсегда.");
            }

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