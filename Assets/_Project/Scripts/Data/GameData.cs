using UnityEngine;

[System.Serializable]
public class GameData
{
    [Header("Параметры игрока")]
    public float maxOxygen = 100f;
    public float currentOxygen;

    [Header("Сохраненный прогресс")]
    public string lastSavedScene = "";

    // Сюда в будущем можно добавлять любые переменные для сохранения:
    // public int health = 100;
    // public int credits = 0;
    // public List<string> inventory;

    public GameData()
    {
        // Инициализация при начале новой игры
        currentOxygen = maxOxygen;
        lastSavedScene = ""; // По умолчанию пусто
    }
}
