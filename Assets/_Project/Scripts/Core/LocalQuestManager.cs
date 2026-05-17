using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections.Generic;

public class LocalQuestManager : MonoBehaviour
{
    [Header("Настройки UI")]
    public TextMeshProUGUI questTextUI;

    public static LocalQuestManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

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
        GameObject foundObject = GameObject.Find("QuestText");
        
        if (foundObject != null)
        {
            questTextUI = foundObject.GetComponent<TextMeshProUGUI>();
        }
    }

    private bool dumpDonePrev;
    private bool mazeDonePrev;
    private bool bossDonePrev;
    private bool allDonePrev;
    private bool initializedQuestState;

    void Update()
    {
        if (questTextUI == null) return;

        if (PlayerController.Instance == null || DataManager.Instance == null || DataManager.Instance.gameData == null)
        {
            questTextUI.text = "Загрузка...";
            return;
        }

        CheckQuestProgress();
        DrawQuestList();
    }

    private void CheckQuestProgress()
    {
        bool dumpDone = IsDumpCleared();
        bool mazeDone = IsMazeCleared();
        bool bossDone = IsBossDefeated();
        bool allDone = dumpDone && mazeDone && bossDone;

        if (!initializedQuestState)
        {
            dumpDonePrev = dumpDone;
            mazeDonePrev = mazeDone;
            bossDonePrev = bossDone;
            allDonePrev = allDone;
            initializedQuestState = true;
            return;
        }

        if (dumpDone && !dumpDonePrev) SoundManager.PlayQuestComplete();
        if (mazeDone && !mazeDonePrev) SoundManager.PlayQuestComplete();
        if (bossDone && !bossDonePrev) SoundManager.PlayQuestComplete();
        if (allDone && !allDonePrev) SoundManager.PlayQuestComplete();

        dumpDonePrev = dumpDone;
        mazeDonePrev = mazeDone;
        bossDonePrev = bossDone;
        allDonePrev = allDone;
    }

    void DrawQuestList()
    {
        StringBuilder sb = new StringBuilder();
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "SpaceShip" || sceneName == "Nimbus")
        {
            sb.AppendLine("<color=#00E5FF>ГЛОБАЛЬНЫЕ ЦЕЛИ:</color>");
            sb.AppendLine(GetTaskStatus("Пройти свалку", IsDumpCleared())); 
            sb.AppendLine(GetTaskStatus("Пройти лабиринт", IsMazeCleared()));
            sb.AppendLine(GetTaskStatus("Одолеть босса", IsBossDefeated()));
            bool readyToRepair = IsDumpCleared() && IsMazeCleared() && IsBossDefeated();
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
            sb.AppendLine(GetTaskStatus("Одолеть босса", IsBossDefeated()));
            sb.AppendLine(FormatResource("Часть двигателя", "engine_part", 1)); 
        }
        else
        {
            sb.AppendLine("Исследуйте территорию");
        }

        questTextUI.text = sb.ToString();
    }


    public bool IsDumpCleared()
    {
        return GetCount("patch") >= 8 && 
               GetCount("coil") >= 5 && 
               GetCount("cable") >= 4 && 
               GetCount("coolant") >= 2 && 
               GetCount("bearing") >= 2;
    }

    public bool IsMazeCleared()
    {
        return GetCount("chip") >= 1;
    }

    bool IsBossDefeated()
    {
        return GetCount("engine_part") >= 1; 
    }


    string FormatResource(string label, string id, int required)
    {
        int current = GetCount(id);
        if (current >= required)
        {
            return $"<color=green>[X] {label}: {current}/{required}</color>";
        }
        return $"[ ] {label}: {current}/{required}";
    }

    int GetCount(string id)
    {
        int totalCount = 0;
        var data = DataManager.Instance.gameData;

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