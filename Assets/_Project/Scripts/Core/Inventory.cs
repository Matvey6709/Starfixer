using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool isChest = false;

    public List<Item> items;

    void Start()
    {
        if (isChest)
            items = DataManager.Instance.gameData.chestInventory;
        else
            items = DataManager.Instance.gameData.inventory;
    }

    public void AddItem(Item newItem)
    {
        foreach (var item in items)
        {
            if (item.id == newItem.id)
            {
                item.amount += newItem.amount;
                return;
            }
        }

        items.Add(newItem);
    }

    public void Clear()
    {
        items.Clear();
        Debug.Log("Инвентарь очищен");
    }
}