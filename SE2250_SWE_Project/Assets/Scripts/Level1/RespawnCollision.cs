using UnityEngine;

public class RespawnCollision : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // Assign in inspector

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Mountain"))
        {
            Respawn();
        }
    }

    void Respawn()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;
        Debug.Log("Player respawned at start.");
    }
}