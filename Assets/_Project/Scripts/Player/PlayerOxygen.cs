using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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

        string currentScene = SceneManager.GetActiveScene().name;

        // Проверяем, на какой мы сцене. Если это "Nimbus", тратим воздух
        if (currentScene == "Nimbus" || currentScene == "Dump")
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

    private bool isDying = false;

    private void DieFromSuffocation()
    {
        if (isDying) return;
        isDying = true;

        // Запускаем последовательность смерти
        StartCoroutine(DeathSequence());
    }

    public void TakeDamage(float damageAmount)
    {
        if (DataManager.Instance == null || isDying) return;

        // Вычитаем фиксированное количество единиц кислорода
        DataManager.Instance.gameData.currentOxygen -= damageAmount;

        // Ограничение, чтобы значение не уходило в минус
        if (DataManager.Instance.gameData.currentOxygen <= 0)
        {
            DataManager.Instance.gameData.currentOxygen = 0;
            DieFromSuffocation();
        }

        UpdateUI();
        Debug.Log($"Получен урон: {damageAmount} ед. Осталось: {DataManager.Instance.gameData.currentOxygen}");
    }

    private IEnumerator DeathSequence()
    {
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetDeadState(true);

            Rigidbody2D rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;        // Полностью гасим инерцию 
                rb.bodyType = RigidbodyType2D.Kinematic; // Отключаем влияние гравитации и внешних сил
            }

            // 2. Отключаем основной коллайдер, чтобы его не выталкивало из текстур пола
            Collider2D col = PlayerController.Instance.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = false;
            }

            Animator anim = PlayerController.Instance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetTrigger("Die");
            }
        }

        Debug.Log("Проигрывание анимации смерти...");

        // Ждем завершения анимации 
        yield return new WaitForSeconds(2.0f);

        Debug.Log("Вам не хватило кислорода... Перезапуск уровня.");

        var data = DataManager.Instance?.gameData;
        bool validCheckpoint = data != null &&
                               data.hasCheckpoint &&
                               !string.IsNullOrEmpty(data.checkpointScene);

        if (GameManager.Instance != null)
        {
            if (validCheckpoint)
            {
                GameManager.Instance.pendingSpawnType = GameManager.SpawnType.Checkpoint;
                GameManager.Instance.RespawnOnScene(data.checkpointScene);
            }
            else
            {
                GameManager.Instance.pendingSpawnType = GameManager.SpawnType.Default;
                GameManager.Instance.RespawnOnScene("SpaceShip");
            }
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
        isDying = false;
        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetDeadState(false);
            Animator anim = PlayerController.Instance.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("idle_down");

                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 0);
                anim.SetBool("IsWalking", false);
            }
        }
            
        FindOxygenSlider();
        UpdateUI();
    }
}
