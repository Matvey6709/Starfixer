using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

// Этот скрипт требует, чтобы на объекте был компонент Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Настройки Передвижения")]
    // Скорость движения, которую можно менять в редакторе
    public float moveSpeed = 5f;

    // Ссылки на нужные компоненты
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;
    private static PlayerController instance;

    private bool isFacingRight = false;
    private bool isInComputerZone = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this) 
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start вызывается один раз перед первым кадром
    void Start()
    {
        // Находим и сохраняем компонент Rigidbody2D, прикрепленный к этому объекту
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
    }

    // Update вызывается каждый кадр. Здесь мы считываем ввод (клавиши)
    void Update()
    {
        // Считываем ввод (это у тебя уже есть)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // Эти строки заставят аниматор "знать", куда мы жмем
        // Получаем текущий масштаб
        anim.SetFloat("Horizontal", moveInput.x);
        anim.SetFloat("Vertical", moveInput.y);

        // Логика включения анимации движения 
        bool isMoving = (moveInput.magnitude > 0);
        anim.SetBool("IsWalking", isMoving);

        // --- ЛОГИКА ПОВОРОТА (FLIP) ---

        // Если жмем ВПРАВО (x > 0) и персонаж смотрит ВЛЕВО
        if (moveInput.x > 0 && !isFacingRight)
            Flip();
        // Если жмем ВЛЕВО (x < 0) и персонаж смотрит ВПРАВО
        else if (moveInput.x < 0 && isFacingRight)
            Flip();

        if (isInComputerZone && Input.GetKeyDown(KeyCode.E))
        {
            ExecuteComputerAction();
        }
    }

    // FixedUpdate вызывается фиксированное количество раз в секунду (для физики)
    void FixedUpdate()
    {
        // Двигаем Rigidbody2D, прикладывая силу (или меняя позицию).
        // Это более правильный способ для персонажа, взаимодействующего с миром.
        // rb.position - текущая позиция.
        // (moveInput * moveSpeed * Time.fixedDeltatime) - смещение за этот физический такт.
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    // Отдельная функция для поворота
    void Flip()
    {
        // Меняем логическое состояние
        isFacingRight = !isFacingRight;

        // Получаем и инвертируем масштаб
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    // Вспомогательный метод для действий компьютера
    void ExecuteComputerAction()
    {
        Debug.Log("Вход в систему ОСИРИС... Авторизация инженера Эрика подтверждена.");
        // Сюда в будущем добавим вызов окна диалога или интерфейса
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Computer"))
        {
            isInComputerZone = true;
            Debug.Log("Подойдите ближе к ОСИРИСУ. Нажмите E для входа.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Computer"))
        {
            isInComputerZone = false;
            Debug.Log("Связь с ОСИРИСОМ потеряна.");
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var vcam = Object.FindAnyObjectByType<CinemachineCamera>(FindObjectsInactive.Include);

        if (vcam != null)
        {
            vcam.Follow = transform;
            vcam.LookAt = transform;
        }

        if (scene.name == "Nimbus") 
        {
            GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
            if (spawnPoint != null)
            {
                transform.position = spawnPoint.transform.position;
                Debug.Log("Удачной вылазки!");
            }
        }
        else if (scene.name == "SpaceShip")
        {
            GameObject homeSpawm = GameObject.FindWithTag("HomeSpawn");
            if (homeSpawm != null)
            {
                transform.position = homeSpawm.transform.position;
                Debug.Log("Добро пожаловать на борт!");
            }
        }
    }
}