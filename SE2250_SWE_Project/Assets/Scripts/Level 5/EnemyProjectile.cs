using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //using health imports from other levels to create project
        }

        Destroy(gameObject);
    }
}
