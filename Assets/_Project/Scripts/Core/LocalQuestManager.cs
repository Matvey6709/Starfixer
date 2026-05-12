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
        // Ищем текст по имени, которое мы видели на твоем скриншоте
        GameObject foundObject = GameObject.Find("QuestText");
        
        if (foundObject != null)
        {
            questTextUI = foundObject.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (questTextUI == null) return;

        if (PlayerController.Instance == null)
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

    // --- ГЛАВНОЕ ИЗМЕНЕНИЕ: ПОДСЧЕТ ИЗ ДВУХ ИСТОЧНИКОВ ---
    int GetCount(string id)
    {
        int totalCount = 0;

        // 1. Считаем в инвентаре игрока
        if (PlayerController.Instance.inventory != null && PlayerController.Instance.inventory.items != null)
        {
            foreach (var item in PlayerController.Instance.inventory.items)
            {
                if (item != null && item.id == id) totalCount += item.amount;
            }
        }

        // 2. Считаем в сундуке (складе)
        // ВАЖНО: Если скрипт сундука называется по-другому, замени 'Inventory' на его имя
        // Мы ищем объект Chest_Warehouse и берем его данные
        var warehouse = GameObject.Find("Chest_Warehouse");
        if (warehouse != null)
        {
            // Здесь мы предполагаем, что на сундуке висит такой же скрипт Inventory или похожий
            // Если у тебя там другой скрипт, замени GetComponent<Inventory>()
            var chestInv = warehouse.GetComponent<Inventory>(); 
            if (chestInv != null && chestInv.items != null)
            {
                foreach (var item in chestInv.items)
                {
                    if (item != null && item.id == id) totalCount += item.amount;
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