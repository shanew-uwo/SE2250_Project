using UnityEngine;

public class HatFollowPlayer : MonoBehaviour
{
    [SerializeField] private float floatHeight = 1.2f;       // Lowered height so it looks like a real hat
    [SerializeField] private float floatAmplitude = 0.05f;   // Gentle bobbing
    [SerializeField] private float floatFrequency = 2f;      // Speed of bobbing

    private Transform playerTransform;
    private Vector3 initialOffset;
    private bool isEquipped = false;

    void Start()
    {
        initialOffset = new Vector3(0, floatHeight, 0);
    }

    void Update()
    {
        if (isEquipped && playerTransform != null)
        {
            float bob = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            Vector3 newPosition = playerTransform.position + initialOffset + new Vector3(0, bob, 0);
            transform.position = newPosition;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isEquipped && other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            isEquipped = true;

            // Disable collider to avoid retriggers
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // Optional: Disable physics
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }
}