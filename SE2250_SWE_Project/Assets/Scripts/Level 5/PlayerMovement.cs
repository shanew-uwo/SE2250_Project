using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Vector3 movementInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        movementInput = new Vector3(moveX, 0, moveZ).normalized;
    }

    void FixedUpdate()
    {
        Vector3 moveVelocity = movementInput * moveSpeed;
        Vector3 newPosition = rb.position + moveVelocity * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    public bool IsMoving()
    {
        return movementInput != Vector3.zero;
    }
}
