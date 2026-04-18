using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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
        }
    }

    // Универсальный метод перезапуска текущей сцены
    public void RestartScene()
    {
        Debug.Log("Перезапуск текущей сцены...");

        // При перезапуске из-за смерти восстанавливаем игроку воздух:
        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData.currentOxygen = DataManager.Instance.gameData.maxOxygen;
            DataManager.Instance.SaveData(); // Сохраняем это изменение
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Метод для возрождения на конкретной сцене по имени
    public void RespawnOnScene(string sceneName)
    {
        Debug.Log($"Возрождение на сцене {sceneName}...");

        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData.currentOxygen = DataManager.Instance.gameData.maxOxygen;
            DataManager.Instance.gameData.lastSavedScene = sceneName;
            DataManager.Instance.SaveData();
        }

        SceneManager.LoadScene(sceneName);
    }

    // Универсальный метод для загрузки новой сцены
    public void LoadNextScene(string sceneName)
    {
        Debug.Log($"Переход на сцену: {sceneName}");
        
        if (DataManager.Instance != null)
        {
           // Обновляем текущую сохраненную сцену
            DataManager.Instance.gameData.lastSavedScene = sceneName;
            
            // Обязательно сохраняем данные при переходе между сценами
            DataManager.Instance.SaveData();
        }
        
      SceneManager.LoadScene(sceneName);
    }

    // Вызови этот метод (например, с кнопки главного меню), чтобы продолжить игру(метод)
    public void ContinueGame()
    {
        if (DataManager.Instance != null && !string.IsNullOrEmpty(DataManager.Instance.gameData.lastSavedScene))
        {
            Debug.Log($"Продолжаем игру со сцены: {DataManager.Instance.gameData.lastSavedScene}");
            SceneManager.LoadScene(DataManager.Instance.gameData.lastSavedScene);
        }
        else
        {
            Debug.LogWarning("Сохранений нет. Запускаем новую игру (здесь впиши имя своей первой сцены)!");
            // SceneManager.LoadScene("НазваниеТвоейПервойСценыКлассическое");
        }
    }
}
