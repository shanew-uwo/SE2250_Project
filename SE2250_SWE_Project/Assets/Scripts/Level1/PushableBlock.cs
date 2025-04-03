using UnityEngine;

public class PushableBlock : MonoBehaviour
{
    public float assistForce = 4f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = new Vector3(playerRb.linearVelocity.x, 0, playerRb.linearVelocity.z);
                rb.AddForce(pushDirection * assistForce, ForceMode.Force);
            }
        }
    }
}