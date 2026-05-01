using UnityEngine;

public class OxygenKapsula : MonoBehaviour
{
    private Animator hatchAnimator;
    private bool isPlayerInRange = false; 
    public bool isFull = true; 

    [Header("Настройки капсулы")]
    public float rechargeTime = 15f; // Время до перезарядки в секундах
    private float currentTimer = 0f;

    void Start()
    {
        hatchAnimator = GetComponent<Animator>();
        if (hatchAnimator == null)
        {
            Debug.LogError("На объекте капсула не найден компонент Animator!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (isFull)
            {
                Debug.Log("Вы рядом с капсулой. Нажмите E для пополнения запаса кислорода.");
            }
            else
            {
                Debug.Log($"Капсула пуста. До заполнения: {Mathf.Ceil(rechargeTime - currentTimer)} сек.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    void Update()
    {
        // Логика перезарядки
        if (!isFull)
        {
            currentTimer += Time.deltaTime;
            if (currentTimer >= rechargeTime)
            {
                Recharge();
            }
        }

        // Взаимодействие
        if (isPlayerInRange && isFull && Input.GetKeyDown(KeyCode.E))
        {
            Open();
        }
    }

    private void Open()
    {
        if (hatchAnimator != null)
        {
            isFull = false;
            currentTimer = 0f;
            hatchAnimator.SetBool("IsFull", false); // Анимация открытия
            
            // Восполняем кислород игроку
            if (PlayerController.Instance != null)
            {
                PlayerOxygen playerOxygen = PlayerController.Instance.GetComponent<PlayerOxygen>();
                if (playerOxygen != null)
                {
                    playerOxygen.RefillOxygen();
                    Debug.Log("Запас кислорода пополнен!");

                    if (DataManager.Instance != null)
                    {
                        var gd = DataManager.Instance.gameData;
                        gd.hasCheckpoint = true;
                        gd.checkpointScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                        gd.checkpointPosition = PlayerController.Instance.transform.position;
                        gd.checkpointInventory = gd.inventory.ConvertAll(i => new Item { id = i.id, itemName = i.itemName, amount = i.amount });
                        gd.checkpointChestInventory = gd.chestInventory.ConvertAll(i => new Item { id = i.id, itemName = i.itemName, amount = i.amount });
                        DataManager.Instance.SaveData();
                        Debug.Log($"Чекпоинт сохранён: сцена={DataManager.Instance.gameData.checkpointScene}, позиция={transform.position}");
                    }
                }
                else
                {
                    Debug.LogError("Компонент PlayerOxygen не найден на игроке!");
                }
            }
            else
            {
                Debug.LogError("PlayerController не найден на сцене!");
            }
        }
    }

    private void Recharge()
    {
        isFull = true;
        currentTimer = 0f;
        if (hatchAnimator != null)
        {
            hatchAnimator.SetBool("IsFull", true); // Анимация закрытия/готовности
        }
        Debug.Log("Капсула с кислородом снова доступна!");
    }
}