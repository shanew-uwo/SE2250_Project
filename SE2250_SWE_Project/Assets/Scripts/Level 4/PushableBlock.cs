using UnityEngine;

public class PushableBlock_Lvl4 : PushableBlock
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Push(Vector3 force)
    {
        rb.AddForce(force, ForceMode.Force);
    }
}