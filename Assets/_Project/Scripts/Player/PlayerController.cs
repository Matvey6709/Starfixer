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
            }
        }
        else if (scene.name == "SpaceShip")
        {
            GameObject homeSpawm = GameObject.FindWithTag("HomeSpawn");
            if (homeSpawm != null)
            {
                transform.position = homeSpawm.transform.position;
            }
        }
    }
}