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
        // Сериализуем наш объект в строку формата JSON
        string json = JsonUtility.ToJson(gameData);
        // Сохраняем в PlayerPrefs устройства
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
            Debug.Log("Данные игрока загружены.");
        }
        else
        {
            // Если сохранений нет, создаем новые стандартные данные
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
