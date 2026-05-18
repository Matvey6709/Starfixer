using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 3f; 

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerOxygen oxygen = collision.GetComponentInParent<PlayerOxygen>();

        if (oxygen != null)
        {
            oxygen.TakeDamage(5f); 
            Destroy(gameObject);
        }   
    }
}