using UnityEngine;

public class EnemyProjectile_Lvl6 : MonoBehaviour
{
    public float lifeTime = 5f;
    public float damageAmount = 10f;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // This method should be called immediately after instantiating
    public void Launch(Vector3 direction, float speed)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * speed;
        }
        else
        {
            Debug.LogWarning("Rigidbody is missing on projectile.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health health = collision.gameObject.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
        Destroy(gameObject);
    }
}