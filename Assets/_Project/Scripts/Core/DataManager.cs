using UnityEngine;
using System.Collections.Generic;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    [Header("Текущие данные")]
    public GameData gameData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        // Временно подставляем checkpoint-значения, чтобы на диск шло
        // только то состояние, которое было на последней капсуле
        var liveInventory      = gameData.inventory;
        var liveChestInventory = gameData.chestInventory;
        var liveOxygen         = gameData.currentOxygen;
        var liveCollectedItems = gameData.collectedItems; // Живые данные со сцены

        gameData.inventory      = gameData.checkpointInventory;
        gameData.chestInventory = gameData.checkpointChestInventory;
        gameData.currentOxygen  = gameData.checkpointOxygen;
        gameData.collectedItems = gameData.checkpointCollectedItems; // Записываем в файл то, что было зафиксировано капсулой

        string json = JsonUtility.ToJson(gameData);

        // Возвращаем живые данные обратно в игру после сохранения
        gameData.inventory      = liveInventory;
        gameData.chestInventory = liveChestInventory;
        gameData.currentOxygen  = liveOxygen;
        gameData.collectedItems = liveCollectedItems;

        PlayerPrefs.SetString("SaveData", json);
        PlayerPrefs.Save();
        Debug.Log("Данные игрока успешно сохранены!");
    }

    public void LoadData()
    {
        if (PlayerPrefs.HasKey("SaveData"))
        {
            string json = PlayerPrefs.GetString("SaveData");
            gameData = JsonUtility.FromJson<GameData>(json);

            if (gameData == null)
                gameData = new GameData();

            if (gameData.inventory == null)
                gameData.inventory = new List<Item>();

            if (gameData.chestInventory == null)
                gameData.chestInventory = new List<Item>();

            if (gameData.checkpointInventory == null)
                gameData.checkpointInventory = new List<Item>();

            if (gameData.checkpointChestInventory == null)
                gameData.checkpointChestInventory = new List<Item>();

            // --- ДОБАВЛЕНО: Защита от пустых списков при загрузке ---
            if (gameData.collectedItems == null)
                gameData.collectedItems = new List<string>();

            if (gameData.checkpointCollectedItems == null)
                gameData.checkpointCollectedItems = new List<string>();

            // Старые сохранения не имели checkpointOxygen — считаем максимум
            if (gameData.checkpointOxygen <= 0)
                gameData.checkpointOxygen = gameData.maxOxygen;

            Debug.Log("Данные игрока загружены.");
        }
        else
        {
            gameData = new GameData();
            Debug.Log("Созданы новые данные игры (сохранений не найдено).");
        }
    }

    public void RestoreFromCheckpoint()
    {
        // Очищаем и заполняем существующие списки инвентаря, не заменяя ссылки
        RefillList(gameData.inventory, gameData.checkpointInventory);
        RefillList(gameData.chestInventory, gameData.checkpointChestInventory);
        
        // --- ДОБАВЛЕНО: Откат собранных предметов при смерти / перезапуске до капсулы ---
        // Если игрок подобрал что-то ПОСЛЕ капсулы и умер, эти предметы вернутся на сцену
        RefillStringList(gameData.collectedItems, gameData.checkpointCollectedItems);

        gameData.currentOxygen = gameData.checkpointOxygen > 0
            ? gameData.checkpointOxygen
            : gameData.maxOxygen;
        Debug.Log("Данные откатаны до чекпоинта. Собранные после капсулы предметы восстановлены.");
    }

    // Оставлен для обратной совместимости
    public void RestoreInventoryFromCheckpoint() => RestoreFromCheckpoint();

    private void RefillList(List<Item> target, List<Item> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(new Item { id = item.id, itemName = item.itemName, amount = item.amount });
    }

    // --- ДОБАВЛЕНО: Вспомогательный метод для безопасной перезаписи списков ID ---
    private void RefillStringList(List<string> target, List<string> source)
    {
        target.Clear();
        if (source != null)
        {
            target.AddRange(source);
        }
    }

    public void ResetData()
    {
        gameData = new GameData();
        SaveData();
        Debug.Log("Данные сброшены к исходным.");
    }
}