using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.UI;

// Этот скрипт требует, чтобы на объекте был компонент Rigidbody2D
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Настройки Передвижения")]
    public float moveSpeed = 5f;
    [Header("Инвентарь")]
    public Inventory inventory;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator anim;
    private bool isFacingRight = false;
    private bool isDead = false;

    public bool IsDead => isDead;

    public void SetDeadState(bool state)
    {
        isDead = state;
    }

    void Awake()
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (isDead) return;
        HandleMovementInput();
    }
    private void HandleMovementInput() // Управление
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        anim.SetFloat("Horizontal", moveInput.x);
        anim.SetFloat("Vertical", moveInput.y);

        bool isMoving = (moveInput.magnitude > 0);
        anim.SetBool("IsWalking", isMoving);

        if (moveInput.x > 0 && !isFacingRight)
            Flip();
        else if (moveInput.x < 0 && isFacingRight)
            Flip();
    }

    void Flip() //Отзеркаливание игрока
    {
        isFacingRight = !isFacingRight;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }


    void FixedUpdate()
    {
        if (isDead) return;
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    
    void OnEnable() =>
        SceneManager.sceneLoaded += OnSceneLoaded;

    void OnDisable() =>
        SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (this != Instance) return;

        var vcam = Object.FindAnyObjectByType<CinemachineCamera>(FindObjectsInactive.Include);
        if (vcam != null)
        {
            vcam.Follow = transform;
            vcam.LookAt = transform; 
            Debug.Log($"Камера найдена на сцене {scene.name}, цель установлена: {transform.name}");
        }
        else
        {
            Debug.LogWarning("CinemachineCamera НЕ найдена на новой сцене!");
        }

        var spawnType = GameManager.Instance?.pendingSpawnType ?? GameManager.SpawnType.Default;
        if (GameManager.Instance != null)
            GameManager.Instance.pendingSpawnType = GameManager.SpawnType.Default;

        if (spawnType == GameManager.SpawnType.Checkpoint)
        {
            var cpData = DataManager.Instance?.gameData;
            if (cpData != null && cpData.hasCheckpoint)
            {
                transform.position = cpData.checkpointPosition;
                return;
            }
        }

        // Default: SpawnPoint в Nimbus, HomeSpawn в SpaceShip
        if (scene.name == "Nimbus")
        {
            var sp = GameObject.FindWithTag("SpawnPoint");
            if (sp != null) transform.position = sp.transform.position;
        }
        else if (scene.name == "SpaceShip")
        {
            var hs = GameObject.FindWithTag("HomeSpawn");
            if (hs != null) transform.position = hs.transform.position;
            else transform.position = new Vector2(-0.51f, 1.1492f);
        }
        else if (scene.name == "Dump")
        {
            var sp = GameObject.FindWithTag("EnterDump"); 
            if (sp != null) transform.position = sp.transform.position;
        }
    }
}