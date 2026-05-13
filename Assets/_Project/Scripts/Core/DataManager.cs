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
        var liveInventory      = gameData.inventory;
        var liveChestInventory = gameData.chestInventory;
        gameData.inventory      = gameData.checkpointInventory;
        gameData.chestInventory = gameData.checkpointChestInventory;

        string json = JsonUtility.ToJson(gameData);

        gameData.inventory      = liveInventory;
        gameData.chestInventory = liveChestInventory;

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

            Debug.Log("Данные игрока загружены.");
        }
        else
        {
            gameData = new GameData();
            Debug.Log("Созданы новые данные игры (сохранений не найдено).");
        }
    }

    public void RestoreInventoryFromCheckpoint()
    {
        gameData.inventory = DeepCopyItems(gameData.checkpointInventory);
        gameData.chestInventory = DeepCopyItems(gameData.checkpointChestInventory);
        Debug.Log("Инвентарь откатан до чекпоинта.");
    }

    private System.Collections.Generic.List<Item> DeepCopyItems(System.Collections.Generic.List<Item> source)
    {
        var copy = new System.Collections.Generic.List<Item>();
        foreach (var item in source)
            copy.Add(new Item { id = item.id, itemName = item.itemName, amount = item.amount });
        return copy;
    }

    public void ResetData()
    {
        gameData = new GameData();
        SaveData();
        Debug.Log("Данные сброшены к исходным.");
    }
}
