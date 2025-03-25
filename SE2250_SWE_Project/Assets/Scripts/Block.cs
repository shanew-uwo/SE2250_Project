using UnityEngine;

public class Block : MonoBehaviour
{
    public Vector3 Position { get; protected set; }

    // Optional: You can still use the Position to track the block's position, but physics will handle movement
    // This is not needed for physics-based movement but can be useful for other tracking or debugging.
}