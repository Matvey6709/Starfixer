using UnityEngine;

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
    private bool isFacingRight = false;
    // Start вызывается один раз перед первым кадром
    void Start()
    {
        // Находим и сохраняем компонент Rigidbody2D, прикрепленный к этому объекту
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update вызывается каждый кадр. Здесь мы считываем ввод (клавиши)
    void Update()
    {
        // Считываем ввод (это у тебя уже есть)
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        // ПЕРЕДАЕМ ВВОД В АНИМАТОР
        // Эти строки заставят аниматор "знать", куда мы жмем
        // Получаем текущий масштаб
        anim.SetFloat("Horizontal", moveInput.x);
        anim.SetFloat("Vertical", moveInput.y);

        // Логика включения анимации движения (у тебя уже есть)
        bool isMoving = (moveInput.magnitude > 0);
        anim.SetBool("IsWalking", isMoving);

        // --- ЛОГИКА ПОВОРОТА (FLIP) ---

        // Если жмем ВПРАВО (x > 0) и персонаж смотрит ВЛЕВО
        if (moveInput.x > 0 && !isFacingRight)
            Flip();
        // Если жмем ВЛЕВО (x < 0) и персонаж смотрит ВПРАВО
        else if (moveInput.x < 0 && isFacingRight)
            Flip();
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
}