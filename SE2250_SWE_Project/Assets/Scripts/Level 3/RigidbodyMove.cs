using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 180f;
    Rigidbody rb;
    Vector3 moveDirection;
    float turnAmount;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Important: Prevent Rigidbody rotation from physics forces if you control it manually
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");

        moveDirection = transform.forward * moveInput * moveSpeed; // Calculate world direction
        turnAmount = turnInput * turnSpeed; // Calculate turn amount

        // --- Animator control code would go here ---
        // Example: GetComponent<Animator>().SetFloat("Speed", moveInput);
    }

    void FixedUpdate() // Physics calculations go in FixedUpdate
    {
        // Apply Movement
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

        // Apply Rotation
        Quaternion deltaRotation = Quaternion.Euler(Vector3.up * turnAmount * Time.fixedDeltaTime);
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}