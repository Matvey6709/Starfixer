using UnityEngine;

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

        gameData.inventory      = gameData.checkpointInventory;
        gameData.chestInventory = gameData.checkpointChestInventory;
        gameData.currentOxygen  = gameData.checkpointOxygen;

        string json = JsonUtility.ToJson(gameData);

        gameData.inventory      = liveInventory;
        gameData.chestInventory = liveChestInventory;
        gameData.currentOxygen  = liveOxygen;

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
                gameData.inventory = new System.Collections.Generic.List<Item>();

            if (gameData.chestInventory == null)
                gameData.chestInventory = new System.Collections.Generic.List<Item>();

            if (gameData.checkpointInventory == null)
                gameData.checkpointInventory = new System.Collections.Generic.List<Item>();

            if (gameData.checkpointChestInventory == null)
                gameData.checkpointChestInventory = new System.Collections.Generic.List<Item>();

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
        // Очищаем и заполняем существующие списки, не заменяя ссылки —
        // иначе Inventory.items потеряет связь с gameData.inventory
        RefillList(gameData.inventory, gameData.checkpointInventory);
        RefillList(gameData.chestInventory, gameData.checkpointChestInventory);
        gameData.currentOxygen = gameData.checkpointOxygen > 0
            ? gameData.checkpointOxygen
            : gameData.maxOxygen;
        Debug.Log("Данные откатаны до чекпоинта.");
    }

    // Оставлен для обратной совместимости
    public void RestoreInventoryFromCheckpoint() => RestoreFromCheckpoint();

    private void RefillList(System.Collections.Generic.List<Item> target, System.Collections.Generic.List<Item> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(new Item { id = item.id, itemName = item.itemName, amount = item.amount });
    }

    public void ResetData()
    {
        gameData = new GameData();
        SaveData();
        Debug.Log("Данные сброшены к исходным.");
    }
}
