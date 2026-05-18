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
    private Collider2D laserCollider; 
    private Transform player;
    private bool isFiring = false;

    void Start()
    {
        if (visual != null)
        {
            laserSprite = visual.GetComponent<SpriteRenderer>();
            laserCollider = visual.GetComponent<Collider2D>();

            if (laserCollider != null) laserCollider.enabled = false;
            visual.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isFiring && laserSprite != null && laserSprite.color.a >= 0.9f && Time.time >= nextDamageTime)
        {
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
        if (laserCollider != null) laserCollider.enabled = false; 

        float elapsed = 0;
        SetLaserAlpha(0.2f);

        while (elapsed < preparationTime)
        {
            UpdateRotation();
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        SetLaserAlpha(1.0f);

        if (laserCollider != null) laserCollider.enabled = true;

        while (elapsed < activeTime)
        {
            UpdateRotation();
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (laserCollider != null) laserCollider.enabled = false;
        visual.SetActive(false);
        isFiring = false;
    }

    void UpdateRotation()
    {
        if (player == null) return;

        if (transform.parent != null)
        {
            Vector3 localPlayerPos = transform.parent.InverseTransformPoint(player.position);
            Vector3 localLaserPos = transform.parent.InverseTransformPoint(transform.position);

            Vector2 localDir = localPlayerPos - localLaserPos;

            float localTargetAngle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg;

            localTargetAngle = Mathf.Clamp(localTargetAngle, -15f, 15f);

            Quaternion targetLocalRotation = Quaternion.Euler(0, 0, localTargetAngle);

            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetLocalRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
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