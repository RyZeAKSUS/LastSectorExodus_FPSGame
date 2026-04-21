using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 25f;
    public float lifetime = 3f;
    public GameObject impactEffect;
    public GameObject bloodEffect;
    public GameObject bulletHolePrefab;

    [Header("Buraco de Bala")]
    public float bulletHoleSize = 0.05f;

    [Header("Sons de Impacto")]
    public AudioClip[] fleshImpactSounds;
    public AudioClip surfaceImpactSound;
    public AudioClip hitMarkerSound;

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
                GameObject effect = Instantiate(
                    bloodEffect,
                    contact.point,
                    Quaternion.LookRotation(contact.normal)
                );
                Destroy(effect, 2f);
            }

            PlayFleshSound(collision.contacts[0].point);
            Destroy(gameObject);
            return;
        }

        ContactPoint surfaceContact = collision.contacts[0];

        if (impactEffect != null)
        {
            GameObject effect = Instantiate(
                impactEffect,
                surfaceContact.point,
                Quaternion.LookRotation(surfaceContact.normal)
            );
            Destroy(effect, 2f);
        }

        if (bulletHolePrefab != null)
        {
            Vector3 holePos = surfaceContact.point + surfaceContact.normal * 0.001f;
            Quaternion holeRot = Quaternion.LookRotation(-surfaceContact.normal);
            GameObject hole = Instantiate(bulletHolePrefab, holePos, holeRot);
            hole.transform.localScale = Vector3.one * bulletHoleSize;
            hole.transform.SetParent(collision.transform);
            Destroy(hole, 30f);
        }

        PlaySurfaceSound(surfaceContact.point);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (_hasHit) return;

        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, speed * Time.fixedDeltaTime * 2f))
        {
            if (_hasHit) return;
            _hasHit = true;

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);

                if (bloodEffect != null)
                {
                    GameObject effect = Instantiate(
                        bloodEffect,
                        hit.point,
                        Quaternion.LookRotation(hit.normal)
                    );
                    Destroy(effect, 2f);
                }

                PlayFleshSound(hit.point);
                Destroy(gameObject);
                return;
            }

            if (impactEffect != null)
            {
                GameObject effect = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );
                Destroy(effect, 2f);
            }

            if (bulletHolePrefab != null)
            {
                Vector3 holePos = hit.point + hit.normal * 0.001f;
                Quaternion holeRot = Quaternion.LookRotation(-hit.normal);
                GameObject hole = Instantiate(bulletHolePrefab, holePos, holeRot);
                hole.transform.localScale = Vector3.one * bulletHoleSize;
                hole.transform.SetParent(hit.transform);
                Destroy(hole, 30f);
            }

            PlaySurfaceSound(hit.point);
            Destroy(gameObject);
        }
    }

    void PlayFleshSound(Vector3 position)
    {
        if (fleshImpactSounds != null && fleshImpactSounds.Length > 0)
        {
            AudioClip clip = fleshImpactSounds[Random.Range(0, fleshImpactSounds.Length)];
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position);
            }
        }

        if (hitMarkerSound != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                AudioSource.PlayClipAtPoint(hitMarkerSound, cam.transform.position);
            }
        }
    }

    void PlaySurfaceSound(Vector3 position)
    {
        if (surfaceImpactSound != null)
        {
            AudioSource.PlayClipAtPoint(surfaceImpactSound, position);
        }
    }
}