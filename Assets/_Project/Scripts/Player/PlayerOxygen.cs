using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Настройки Кислорода")]
    public float oxygenDepletionRate = 5f; // Сколько кислорода тратится в секунду на вылазке
    public Slider oxygenSlider; // автоматически привязывается UI Slider сюда

    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {
        if (PlayerController.Instance != null && PlayerController.Instance.IsDead) return;

        HandleOxygenLogic();
    }

    private void HandleOxygenLogic()
    {
        // Убедимся, что DataManager существует на сцене
        if (DataManager.Instance == null) return;

        // Проверяем, на какой мы сцене. Если это "Nimbus", тратим воздух
        if (SceneManager.GetActiveScene().name == "Nimbus")
        {
            if (DataManager.Instance.gameData.currentOxygen > 0)
            {
                DataManager.Instance.gameData.currentOxygen -= oxygenDepletionRate * Time.deltaTime;
                UpdateUI();

                if (DataManager.Instance.gameData.currentOxygen <= 0)
                {
                    DieFromSuffocation();
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (oxygenSlider == null)
        {
            FindOxygenSlider();
        }

        if (oxygenSlider != null && DataManager.Instance != null)
        {
            oxygenSlider.value = DataManager.Instance.gameData.currentOxygen / DataManager.Instance.gameData.maxOxygen;
        }
    }

    private void FindOxygenSlider()
    {
        GameObject sliderObj = GameObject.FindWithTag("OxygenSlider");
        if (sliderObj != null)
        {
            oxygenSlider = sliderObj.GetComponent<Slider>();
        }
    }

    public void RefillOxygen()
    {
        if (DataManager.Instance != null)
        {
            DataManager.Instance.gameData.currentOxygen = DataManager.Instance.gameData.maxOxygen;
            UpdateUI();
            Debug.Log("Кислород полностью восстановлен!");
        }
    }

    private void DieFromSuffocation() // Вызывается, если игрок погиб
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetDeadState(true); // Блокируем управление

        Debug.Log("Вам не хватило кислорода... Перезапуск уровня.");

        // Оригинальная логика: почти сразу сбрасывался стейт смерти
        if (PlayerController.Instance != null)
            PlayerController.Instance.SetDeadState(false);

        // Используем GameManager для перезапуска сцены (он также сохранит данные)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RespawnOnScene("SpaceShip");
        }
        else 
        {
            SceneManager.LoadScene("SpaceShip");
        }
    }

    void OnEnable() =>
        SceneManager.sceneLoaded += OnSceneLoaded;

    void OnDisable() =>
        SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Автоматически находим слайдер на новой сцене
        FindOxygenSlider();
        
        // Обновляем UI на новой сцене с актуальными данными
        UpdateUI();
    }
}
