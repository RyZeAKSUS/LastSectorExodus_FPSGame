using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;
    public float damage = 25f;
    public float lifetime = 3f;
    public GameObject impactEffect;
    public GameObject bloodEffect;

    [Header("Sons de Impacto")]
    public AudioClip[] fleshImpactSounds;
    public AudioClip surfaceImpactSound;
    public AudioClip hitMarkerSound;

    [Header("Headshot")]
    public float headshotMultiplier = 2.5f;
    public LayerMask headshotLayer;

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

    void OnTriggerEnter(Collider other)
    {
        if (_hasHit) return;

        HeadshotZone zone = other.GetComponent<HeadshotZone>();
        if (zone == null) return;
        if (zone.enemyHealth == null) return;

        _hasHit = true;

        float headshotDamage = damage * headshotMultiplier;
        zone.enemyHealth.TakeDamage(headshotDamage, true);

        if (bloodEffect != null)
        {
            GameObject effect = Instantiate(
                bloodEffect,
                transform.position,
                Quaternion.identity
            );
            Destroy(effect, 2f);
        }

        PlayFleshSound(transform.position);
        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (_hasHit) return;
        _hasHit = true;

        EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage, false);

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

        PlaySurfaceSound(surfaceContact.point);
        Destroy(gameObject);
    }

    void FixedUpdate()
    {
        if (_hasHit) return;

        float checkDistance = speed * Time.fixedDeltaTime * 2f;

        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit headshotHit, checkDistance, headshotLayer,
            QueryTriggerInteraction.Collide))
        {
            if (_hasHit) return;
            _hasHit = true;

            HeadshotZone zone = headshotHit.collider.GetComponent<HeadshotZone>();
            if (zone != null && zone.enemyHealth != null)
            {
                float headshotDamage = damage * headshotMultiplier;
                zone.enemyHealth.TakeDamage(headshotDamage, true);

                if (bloodEffect != null)
                {
                    GameObject effect = Instantiate(
                        bloodEffect,
                        headshotHit.point,
                        Quaternion.LookRotation(headshotHit.normal)
                    );
                    Destroy(effect, 2f);
                }

                PlayFleshSound(headshotHit.point);
                Destroy(gameObject);
                return;
            }
        }

        if (Physics.Raycast(transform.position, transform.forward,
            out RaycastHit hit, checkDistance))
        {
            if (_hasHit) return;
            _hasHit = true;

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, false);

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