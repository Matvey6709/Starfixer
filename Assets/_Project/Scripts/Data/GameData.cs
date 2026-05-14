using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    [Header("Параметры игрока")]
    public float maxOxygen = 100f;
    public float currentOxygen;

    [Header("Чекпоинт (последняя капсула)")]
    public bool hasCheckpoint = false;
    public string checkpointScene = "";
    public Vector2 checkpointPosition;
    public float checkpointOxygen;
    public List<Item> checkpointInventory = new List<Item>();
    public List<Item> checkpointChestInventory = new List<Item>();


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
        currentOxygen = maxOxygen;
        checkpointOxygen = maxOxygen;
        inventory = new List<Item>();
        chestInventory = new List<Item>();
        checkpointInventory = new List<Item>();
        checkpointChestInventory = new List<Item>();
    }
}





