using UnityEngine;

public class BlockController : MonoBehaviour
{
    public MovableBlock movableBlock;

    private void Update()
    {
        if (movableBlock != null)
        {
            // Get input and move the block accordingly.
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 direction = new Vector3(horizontal, 0, vertical);
            movableBlock.Move(direction);
        }
    }
}