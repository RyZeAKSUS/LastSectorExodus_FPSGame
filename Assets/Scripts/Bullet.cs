using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 25f;
    public float lifetime = 3f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        rb.useGravity = false;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
            enemy.TakeDamage(damage);

        Destroy(gameObject);
    }
}