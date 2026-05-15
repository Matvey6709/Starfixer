using UnityEngine;
using System.Collections.Generic;

public class ItemPickup : MonoBehaviour
{
    public Item item; // Данные предмета (ID: chip, Name: Chip)

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (DataManager.Instance == null || DataManager.Instance.gameData == null) return;

            List<Item> playerInv = DataManager.Instance.gameData.inventory;
            bool found = false;

            // Ищем в глобальном списке
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

            Debug.Log($"Чип добавлен в DataManager!");
            Destroy(gameObject); // Исчезает со сцены
        }
    }
}