using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections.Generic;

public class LocalQuestManager : MonoBehaviour
{
    [Header("Настройки UI")]
    public TextMeshProUGUI questTextUI;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ищем текст по имени при загрузке новой локации
        GameObject foundObject = GameObject.Find("QuestText");
        
        if (foundObject != null)
        {
            questTextUI = foundObject.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (questTextUI == null) return;

        // Добавил проверку на DataManager, чтобы не было ошибок при старте
        if (PlayerController.Instance == null || DataManager.Instance == null)
        {
            questTextUI.text = "Загрузка...";
            return;
        }

        DrawQuestList();
    }

    void DrawQuestList()
    {
        StringBuilder sb = new StringBuilder();
        string sceneName = SceneManager.GetActiveScene().name;

        // --- ЛОГИКА ОТОБРАЖЕНИЯ ПО СЦЕНАМ ---

        if (sceneName == "SpaceShip" || sceneName == "Nimbus")
        {
            sb.AppendLine("<color=#00E5FF>ГЛОБАЛЬНЫЕ ЦЕЛИ:</color>");
            sb.AppendLine(GetTaskStatus("Пройти свалку", false)); 
            sb.AppendLine(GetTaskStatus("Пройти лабиринт", false));
            sb.AppendLine(GetTaskStatus("Одолеть босса", false));
            sb.AppendLine(GetTaskStatus("Починить корабль", false));
        }
        else if (sceneName == "Dump")
        {
            sb.AppendLine("<color=#FFD700>ЗАДАЧИ: СВАЛКА</color>");
            sb.AppendLine(FormatResource("Изолента", "patch", 8));
            sb.AppendLine(FormatResource("Катушка", "coil", 5));
            sb.AppendLine(FormatResource("Кабель", "cable", 4));
            sb.AppendLine(FormatResource("Охлаждайка", "coolant", 2));
            sb.AppendLine(FormatResource("Подшипник", "bearing", 2));
        }
        else if (sceneName == "Maze")
        {
            sb.AppendLine("<color=#FF00FF>ЗАДАЧИ: ЛАБИРИНТ</color>");
            sb.AppendLine(FormatResource("Микросхема", "chip", 1));
        }
        else if (sceneName == "Boss")
        {
            sb.AppendLine("<color=#FF4500>ФИНАЛЬНЫЙ ЭТАП:</color>");
            sb.AppendLine("- Одолеть босса");
            sb.AppendLine(FormatResource("Часть двигателя", "engine_part", 1));
        }
        else
        {
            sb.AppendLine("Исследуйте территорию");
        }

        questTextUI.text = sb.ToString();
    }

    // Вспомогательная функция для оформления строк ресурсов
    string FormatResource(string label, string id, int required)
    {
        int current = GetCount(id);
        if (current >= required)
        {
            return $"<color=green>[X] {label}: {current}/{required}</color>";
        }
        return $"[ ] {label}: {current}/{required}";
    }

    // --- ЕДИНЫЙ ЦЕНТР ПОДСЧЕТА ЧЕРЕЗ DATAMANAGER ---
    int GetCount(string id)
    {
        int totalCount = 0;

        // Проверяем, существует ли база данных
        if (DataManager.Instance == null || DataManager.Instance.gameData == null) 
            return 0;

        var data = DataManager.Instance.gameData;

        // 1. Ищем в глобальном инвентаре игрока
        if (data.inventory != null)
        {
            foreach (var item in data.inventory)
            {
                if (item != null && string.Equals(item.id, id, System.StringComparison.OrdinalIgnoreCase))
                {
                    totalCount += item.amount;
                }
            }
        }

        // 2. Ищем в глобальном инвентаре сундука
        if (data.chestInventory != null)
        {
            foreach (var item in data.chestInventory)
            {
                if (item != null && string.Equals(item.id, id, System.StringComparison.OrdinalIgnoreCase))
                {
                    totalCount += item.amount;
                }
            }
        }

        return totalCount;
    }

    string GetTaskStatus(string taskName, bool isDone)
    {
        return isDone ? $"<s><color=green>[X] {taskName}</color></s>" : $"[ ] {taskName}";
    }
}