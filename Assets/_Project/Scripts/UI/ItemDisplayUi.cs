using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ItemDisplayUI : MonoBehaviour
{
    [Header("Настройки")]
    public string itemID = "metal"; 
    public bool isChestUI;         

    [Header("Ссылки")]
    public GameObject iconObject;
    public TextMeshProUGUI countText;

    void Update()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null) return;

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

        if (iconObject != null)
            iconObject.SetActive(itemAmount >= 0);
    }
}