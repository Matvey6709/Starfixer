using UnityEngine;
using System.Collections.Generic;
public class InventorySlot : MonoBehaviour

{

    [Header("Настройки слота")]

    [Tooltip("ID должен совпадать с ID предмета в ItemPickup (например: patch, coil, metal)")]

    public string itemID = "metal";


    [Tooltip("Отображаемое имя предмета (используется при создании новой записи)")]

    public string itemName = "Metal";


    [Tooltip("Отметьте, если этот слот находится внутри панели СУНДУКА")]

    public bool isChestSlot;



    [Header("Ссылка на сундук")]

    [Tooltip("Перетащите сюда объект сундука со сцены (у которого скрипт ChestInteraction)")]

    public ChestInteraction chest;

    public void OnClick()

    {

        if (chest != null && !chest.isOpen) return;


        if (DataManager.Instance == null || DataManager.Instance.gameData == null)

        {

            Debug.LogError("InventorySlot: DataManager или GameData не найдены!");

            return;

        }



        var data = DataManager.Instance.gameData;



        if (isChestSlot)

        {

            if (HasItem(data.chestInventory))

            {

                TransferItem(data.chestInventory, data.inventory);

            }

        }

        else

        {

            if (HasItem(data.inventory))

            {

                TransferItem(data.inventory, data.chestInventory);

            }

        }

    }




    private bool HasItem(List<Item> sourceList)

    {
        foreach (var item in sourceList)

        {

            if (item.id == itemID && item.amount > 0) return true;

        }

        return false;

    }

    private void TransferItem(List<Item> from, List<Item> to)

    {
        for (int i = 0; i < from.Count; i++)

        {

            if (from[i].id == itemID)

            {

                from[i].amount--;


                if (from[i].amount <= 0)

                {

                    from.RemoveAt(i);

                }

                break;

            }

        }

        bool foundInTarget = false;

        foreach (var item in to)

        {

            if (item.id == itemID)

            {

                item.amount++;

                foundInTarget = true;

                break;

            }

        }

        if (!foundInTarget)

        {

            to.Add(new Item { id = itemID, itemName = itemName, amount = 1 });

        }


        Debug.Log($"Предмет {itemID} перенесен. Осталось в источнике: {from.Count}");

    }

}