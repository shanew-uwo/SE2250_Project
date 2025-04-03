using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyChase : MonoBehaviour
{
    public Transform player;
    public float moveSpeed = 3f;

    private PlayerMovement playerMovement;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    void FixedUpdate()
    {
        if (player != null && playerMovement != null && playerMovement.IsMoving())
        {
            Vector3 direction = (player.position - transform.position).normalized;
            Vector3 newPos = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       //once git files are merged collison detection can be used to subtract health from player
    }
}
