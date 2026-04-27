using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    [Header("Параметры игрока")]
    public float maxOxygen = 100f;
    public float currentOxygen;

    [Header("Сохраненный прогресс")]
    public string lastSavedScene = "";


    [Header("Инвентарь игрока")]
    public List<Item> inventory = new List<Item>();

    [Header("Инвентарь сундука")]
    public List<Item> chestInventory = new List<Item>();

    // Сюда в будущем можно добавлять любые переменные для сохранения:
    // public int health = 100;
    // public int credits = 0;
    // public List<string> inventory;

    public GameData()
    {
        // Инициализация при начале новой игры
        currentOxygen = maxOxygen;
        lastSavedScene = ""; // По умолчанию пусто
        inventory = new List<Item>(); //создаем пустые инвентари
        chestInventory = new List<Item>();
    }
}





