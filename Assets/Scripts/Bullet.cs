using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 25f;
    public float lifetime = 3f;
    public GameObject impactEffect;
    public GameObject bloodEffect;

    private Rigidbody _rb;
    private bool _hasHit = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.linearVelocity = transform.forward * speed;
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;
        _hasHit = true;

        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (bloodEffect != null)
            {
                ContactPoint contact = collision.contacts[0];
                GameObject effect = Instantiate(bloodEffect, contact.point, Quaternion.LookRotation(contact.normal));
                Destroy(effect, 2f);
            }
            Destroy(gameObject);
            return;
        }

        if (impactEffect != null)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject effect = Instantiate(impactEffect, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(effect, 2f);
        }

        Destroy(gameObject);
    }

     void FixedUpdate()
    {
        if (_hasHit) return;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed * Time.fixedDeltaTime * 2f))
        {
            if (_hasHit) return;
            _hasHit = true;

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                if (bloodEffect != null)
                {
                    GameObject effect = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(effect, 2f);
                }
                Destroy(gameObject);
                return;
            }

            if (impactEffect != null)
            {
                GameObject effect = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(effect, 2f);
            }

            Destroy(gameObject);
        }
    }
}