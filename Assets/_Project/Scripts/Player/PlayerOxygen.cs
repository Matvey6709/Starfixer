using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayerOxygen : MonoBehaviour
{
    [Header("Настройки Кислорода")]
    public float oxygenDepletionRate = 3f; 
    public Slider oxygenSlider; 

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
        if (DataManager.Instance == null) return;

        string currentScene = SceneManager.GetActiveScene().name;

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
            oxygenSlider.interactable = false;
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

        StartCoroutine(DeathSequence());
    }

    public void TakeDamage(float damageAmount)
    {
        if (DataManager.Instance == null || isDying) return;

        DataManager.Instance.gameData.currentOxygen -= damageAmount;

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
        SoundManager.PlayDeath();
        SoundManager.SetFootsteps(false);

        if (PlayerController.Instance != null)
        {
            PlayerController.Instance.SetDeadState(true);

            Rigidbody2D rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;        
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

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

            Collider2D col = PlayerController.Instance.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = true;
                Debug.Log("[ФИЗИКА] Коллайдер игрока успешно восстановлен.");
            }

            Rigidbody2D rb = PlayerController.Instance.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero; 
            }

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
