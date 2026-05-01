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
            LoadData(); // Загружаем данные при самом первом запуске
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        // На диск инвентарь пишется только из снапшота чекпоинта,
        // чтобы предметы, поднятые после чекпоинта, не попадали в сохранение.
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

            // 💥 ВАЖНО: защита от null
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
    public void ResetData()
    {
        gameData = new GameData();
        SaveData();
        Debug.Log("Данные сброшены к исходным.");
    }
}
