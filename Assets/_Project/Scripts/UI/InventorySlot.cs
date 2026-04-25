using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [Header("Настройки")]
    public bool isChestSlot; // это слот сундука или игрока

    [Header("Ссылка на сундук")]
    public ChestInteraction chest; // сюда перетаскиваем сундук

    public void OnClick()
    {
        // ❗ если есть сундук и он закрыт — ничего не делаем
        if (chest != null && !chest.isOpen)
            return;

        var data = DataManager.Instance.gameData;

        // 👉 если это слот сундука → забираем из сундука
        if (isChestSlot)
        {
            if (GetAmount(data.chestInventory) > 0)
            {
                RemoveItem(data.chestInventory);
                AddItem(data.inventory);
            }
        }
        // 👉 если это слот игрока → кладём в сундук
        else
        {
            if (GetAmount(data.inventory) > 0)
            {
                RemoveItem(data.inventory);
                AddItem(data.chestInventory);
            }
        }
    }

    // 🔍 получить количество металла
    int GetAmount(System.Collections.Generic.List<Item> list)
    {
        foreach (var item in list)
        {
            if (item.id == "metal")
                return item.amount;
        }
        return 0;
    }

    // ➖ убрать 1 предмет
    void RemoveItem(System.Collections.Generic.List<Item> list)
    {
        foreach (var item in list)
        {
            if (item.id == "metal")
            {
                item.amount--;
                return;
            }
        }
    }

    // ➕ добавить 1 предмет
    void AddItem(System.Collections.Generic.List<Item> list)
    {
        foreach (var item in list)
        {
            if (item.id == "metal")
            {
                item.amount++;
                return;
            }
        }

        // если предмета нет — создаём
        Item newItem = new Item();
        newItem.id = "metal";
        newItem.itemName = "Metal";
        newItem.amount = 1;

        list.Add(newItem);
    }
}