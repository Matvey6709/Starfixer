using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f; 

    void Start()
    {
        // Пуля летит вперед (в ту сторону, куда направлен FirePoint)
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ищем скрипт на игроке через Hurtbox
        PlayerOxygen oxygen = collision.GetComponentInParent<PlayerOxygen>();

        if (oxygen != null)
        {
            oxygen.TakeDamage(5f); // Фиксированные 5 единиц
            Destroy(gameObject);
        }   
    }
}