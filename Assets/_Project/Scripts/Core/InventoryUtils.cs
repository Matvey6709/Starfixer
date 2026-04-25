using System.Collections.Generic;

public static class InventoryUtils
{
    public static int GetItemAmount(List<Item> inventory, string id)
    {
        foreach (var item in inventory)
        {
            if (item.id == id)
                return item.amount;
        }
        return 0;
    }

    public static void AddItem(List<Item> inventory, string id, int amount)
    {
        foreach (var item in inventory)
        {
            if (item.id == id)
            {
                item.amount += amount;
                return;
            }
        }

        inventory.Add(new Item { id = id, amount = amount });
    }

    public static void RemoveItem(List<Item> inventory, string id, int amount)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].id == id)
            {
                inventory[i].amount -= amount;

                if (inventory[i].amount <= 0)
                    inventory.RemoveAt(i);

                return;
            }
        }
    }
}