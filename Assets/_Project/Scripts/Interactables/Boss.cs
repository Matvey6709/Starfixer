using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public LaserFollow laserScript;

    [Header("Эффекты Смерти")]
    [SerializeField] private GameObject lightningExplosionPrefab; 
    [SerializeField] private Transform arenaCenter;        

    private Unity.Cinemachine.CinemachineImpulseSource impulseSource;

    private Animator anim;
    private float timer;
    private bool isPhaseTwo = false;
    private float phaseMultiplier = 1f;
    private float minY = 3.6f;    
    private float maxY = 7f;
    private float actionInterval = 2f; 
    private float jumpDuration = 1f;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError($"[БОСС] На объекте {gameObject.name} не найден компонент Animator!");
        }

        impulseSource = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("Босс успешно нашел игрока на сцене!");
        }
        else
        {
            Debug.LogError("Босс не может найти игрока! Проверь тег на префабе игрока.");
        }
    }

    void Update()
    {
        if (player == null) return;

        HandleFlip();

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ExecuteComboAction();
            timer = actionInterval * phaseMultiplier;
        }
    }

    void HandleFlip()
    {
        float direction = player.position.x - transform.position.x;
        if (direction > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (direction < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void ExecuteComboAction()
    {
        StartCoroutine(JumpAttack());
        int attackChoice = Random.Range(1, 3);
        if (attackChoice == 1) SimpleShoot();
        else StrongLaser();
    }

    IEnumerator JumpAttack()
    {
        anim.SetTrigger("Jump");
        float targetY = Mathf.Clamp(player.position.y, minY, maxY);
        Vector2 targetPos = new Vector2(transform.position.x, targetY);

        float elapsed = 0;
        Vector2 startPos = transform.position;

        float currentJumpTime = jumpDuration * phaseMultiplier;

        while (elapsed < currentJumpTime)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsed / currentJumpTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
    }

    void SimpleShoot()
    {
        anim.SetTrigger("Shoot");
        Instantiate(bulletPrefab, firePoint.position, GetClampedAimRotation());
        SoundManager.PlayBossShot();
    }

    void StrongLaser()
    {
        anim.SetTrigger("Laser");
        SoundManager.PlayBossLaser();
        if (laserScript != null)
        {
            laserScript.Fire(player);
        }
    }

    Quaternion GetClampedAimRotation()
    {
        Vector2 dir = player.position - firePoint.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        bool facingRight = transform.localScale.x > 0;

        if (facingRight)
        {
            angle = Mathf.Clamp(angle, -15f, 15f);
        }
        else
        {
            if (angle < 0) angle += 360f;
            angle = Mathf.Clamp(angle, 165f, 195f);
        }

        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void ActivatePhaseTwo()
    {
        if (isPhaseTwo) return;
        isPhaseTwo = true;
        phaseMultiplier = 0.5f; 
        anim.speed = 2f;
    }

    [ContextMenu("Test Death")]
    public void Die()
    {
        StopAllCoroutines();
        anim.SetTrigger("Death");

        if (lightningExplosionPrefab != null && arenaCenter != null)
        {
            GameObject explosion = Instantiate(lightningExplosionPrefab, arenaCenter.position, Quaternion.identity);

            Destroy(explosion, 3f);
        }
        else
        {
            Debug.LogWarning("Не назначены Prefab взрыва или Center арены в скрипте Босса!");
        }

        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        this.enabled = false;
    }
}