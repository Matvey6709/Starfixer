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



    /// Метод вызывается при нажатии на кнопку (белый квадрат) в UI

    public void OnClick()

    {

        // Проверка: если сундук назначен, но закрыт — игнорируем клик

        if (chest != null && !chest.isOpen) return;



        // Проверка: есть ли доступ к данным

        if (DataManager.Instance == null || DataManager.Instance.gameData == null)

        {

            Debug.LogError("InventorySlot: DataManager или GameData не найдены!");

            return;

        }



        var data = DataManager.Instance.gameData;



        if (isChestSlot)

        {

            // ЗАБИРАЕМ ИЗ СУНДУКА -> В ИНВЕНТАРЬ ИГРОКА

            if (HasItem(data.chestInventory))

            {

                TransferItem(data.chestInventory, data.inventory);

            }

        }

        else

        {

            // КЛАДЕМ ИЗ ИНВЕНТАРЯ -> В СУНДУК

            if (HasItem(data.inventory))

            {

                TransferItem(data.inventory, data.chestInventory);

            }

        }

    }



    // Проверяет, есть ли хотя бы 1 такой предмет в списке

    private bool HasItem(List<Item> sourceList)

    {

        foreach (var item in sourceList)

        {

            if (item.id == itemID && item.amount > 0) return true;

        }

        return false;

    }



    // Логика переноса 1 единицы предмета

    private void TransferItem(List<Item> from, List<Item> to)

    {

        // 1. Убираем из источника

        for (int i = 0; i < from.Count; i++)

        {

            if (from[i].id == itemID)

            {

                from[i].amount--;


                // Если стало 0 — полностью удаляем из списка данных

                if (from[i].amount <= 0)

                {

                    from.RemoveAt(i);

                }

                break;

            }

        }



        // 2. Добавляем в цель

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



        // Если в целевом списке такого предмета еще не было — создаем новую запись

        if (!foundInTarget)

        {

            to.Add(new Item { id = itemID, itemName = itemName, amount = 1 });

        }


        Debug.Log($"Предмет {itemID} перенесен. Осталось в источнике: {from.Count}");

    }

}