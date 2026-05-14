using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum SpawnType { Default, Checkpoint }

    public SpawnType pendingSpawnType = SpawnType.Default;

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

    public void RestartScene()
    {
        Debug.Log("Перезапуск текущей сцены...");

        if (DataManager.Instance != null)
        {
            DataManager.Instance.RestoreFromCheckpoint();
            DataManager.Instance.SaveData();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RespawnOnScene(string sceneName)
    {
        Debug.Log($"Возрождение на сцене {sceneName}...");

        if (DataManager.Instance != null)
        {
            DataManager.Instance.RestoreFromCheckpoint();
            DataManager.Instance.SaveData();
        }

        SceneManager.LoadScene(sceneName);
    }

    public void LoadNextScene(string sceneName)
    {
        Debug.Log($"Переход на сцену: {sceneName}");

        if (DataManager.Instance != null)
            DataManager.Instance.SaveData();

        SceneManager.LoadScene(sceneName);
    }

    public void ContinueGame()
    {
        if (DataManager.Instance != null && DataManager.Instance.gameData.hasCheckpoint)
        {
            pendingSpawnType = SpawnType.Checkpoint;
            Debug.Log($"Продолжаем игру: возврат к чекпоинту на {DataManager.Instance.gameData.checkpointScene}.");
            SceneManager.LoadScene(DataManager.Instance.gameData.checkpointScene);
        }
        else
        {
            pendingSpawnType = SpawnType.Default;
            Debug.Log("Сохранений нет — начинаем с корабля.");
            SceneManager.LoadScene("SpaceShip");
        }
    }
}
