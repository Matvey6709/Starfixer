using UnityEngine;
using System.Collections;

public class LaserFollow : MonoBehaviour
{
    public float rotationSpeed = 15f;
    public float preparationTime = 0.5f;
    public float activeTime = 0.5f;
    public GameObject visual;

    [Header("Damage Settings")]
    public float damageAmount = 1f;
    public float damageInterval = 0.1f;
    private float nextDamageTime;

    private SpriteRenderer laserSprite;
    private Collider2D laserCollider; // Кэшируем коллайдер
    private Transform player;
    private bool isFiring = false;

    void Start()
    {
        if (visual != null)
        {
            laserSprite = visual.GetComponent<SpriteRenderer>();
            laserCollider = visual.GetComponent<Collider2D>();

            // Сразу выключаем коллайдер и визуал
            if (laserCollider != null) laserCollider.enabled = false;
            visual.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Проверяем фазу через прозрачность
        if (isFiring && laserSprite != null && laserSprite.color.a >= 0.9f && Time.time >= nextDamageTime)
        {
            // Ищем скрипт на главном объекте игрока
            PlayerOxygen oxygen = collision.GetComponentInParent<PlayerOxygen>();
            if (oxygen != null)
            {
                oxygen.TakeDamage(damageAmount);
                nextDamageTime = Time.time + damageInterval;
            }
        }
    }

    public void Fire(Transform target)
    {
        if (target == null) return;
        player = target;
        if (!isFiring) StartCoroutine(LaserRoutine());
    }

    IEnumerator LaserRoutine()
    {
        isFiring = true;
        visual.SetActive(true);
        if (laserCollider != null) laserCollider.enabled = false; // На этапе подготовки коллайдер выключен

        // --- ЭТАП 1: ПОДСКАЗКА ---
        float elapsed = 0;
        SetLaserAlpha(0.2f);

        while (elapsed < preparationTime)
        {
            UpdateRotation();
            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- ЭТАП 2: ВЫСТРЕЛ ---
        elapsed = 0;
        SetLaserAlpha(1.0f);

        // Включаем коллайдер только сейчас!
        if (laserCollider != null) laserCollider.enabled = true;

        while (elapsed < activeTime)
        {
            UpdateRotation();
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Финальное выключение
        if (laserCollider != null) laserCollider.enabled = false;
        visual.SetActive(false);
        isFiring = false;
    }

    void UpdateRotation()
    {
        if (player == null) return;

        // Если у лазера есть родитель (Босс), работаем строго в ЛОКАЛЬНЫХ координатах
        if (transform.parent != null)
        {
            // Переводим мировые позиции игрока и лазера в локальное пространство Босса
            Vector3 localPlayerPos = transform.parent.InverseTransformPoint(player.position);
            Vector3 localLaserPos = transform.parent.InverseTransformPoint(transform.position);

            // Получаем локальное направление от лазера к игроку
            Vector2 localDir = localPlayerPos - localLaserPos;

            // Считаем локальный угол. Благодаря флипу родителя, 
            // "перед" босса всегда соответствует 0 градусов на обеих сторонах!
            float localTargetAngle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

            // Теперь ограничение одинаковое для обеих сторон (например, от -15 до 15 градусов)
            localTargetAngle = Mathf.Clamp(localTargetAngle, -15f, 15f);

            // Создаем локальное вращение
            Quaternion targetLocalRotation = Quaternion.Euler(0, 0, localTargetAngle);

            // Вращаем ЛОКАЛЬНО. Это полностью убирает баг с закручиванием по часовой стрелке
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetLocalRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Запасной вариант на случай, если у объекта почему-то нет родителя (мировой расчет)
            Vector2 dir = player.position - transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            targetAngle = Mathf.Clamp(targetAngle, -15f, 15f);

            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void SetLaserAlpha(float alpha)
    {
        if (laserSprite != null)
        {
            Color c = laserSprite.color;
            c.a = alpha;
            laserSprite.color = c;
        }
    }
}