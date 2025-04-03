using UnityEngine;

public class FloatingGem : MonoBehaviour
{
    public float floatSpeed = 2f;   // Speed of the floating movement
    public float floatHeight = 0.3f;  // How high the gem floats up and down
    public float rotationSpeed = 45f;  // Speed of the rotation (degrees per second)

    private Vector3 startPos;

    void Start()
    {
        // Save the starting position of the gem to apply floating movement
        startPos = transform.position;
    }

    void Update()
    {
        // Floating movement using sine function for smooth up and down motion
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatHeight + startPos.y;

        // Apply the new position to the gem, maintaining its original x and z coordinates
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Continuous rotation
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
