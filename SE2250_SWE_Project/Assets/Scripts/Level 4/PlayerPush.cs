using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    public float pushForce = 5f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        PushableBlock pushable = hit.collider.GetComponent<PushableBlock>();
        if (pushable != null)
        {
            Vector3 pushDirection = hit.moveDirection;
            pushable.Push(pushDirection * pushForce);
        }
    }
}