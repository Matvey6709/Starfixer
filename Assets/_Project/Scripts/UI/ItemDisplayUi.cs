using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ItemDisplayUI : MonoBehaviour
{
    [Header("Настройки")]
    public string itemID = "metal"; // ID предмета для отображения
    public bool isChestUI;          // Галочка: это UI сундука или игрока?

    [Header("Ссылки")]
    public GameObject iconObject;
    public TextMeshProUGUI countText;

    void Update()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null) return;

        // Выбираем нужный список в зависимости от того, чей это UI
        List<Item> targetInventory = isChestUI 
            ? DataManager.Instance.gameData.chestInventory 
            : DataManager.Instance.gameData.inventory;

        int itemAmount = 0;

        foreach (var item in targetInventory)
        {
            if (item.id == itemID)
            {
                itemAmount = item.amount;
                break;
            }
        }

        if (countText != null)
            countText.text = itemAmount.ToString();

        // Скрываем иконку, если предметов 0
        if (iconObject != null)
            iconObject.SetActive(itemAmount >= 0);
    }
}