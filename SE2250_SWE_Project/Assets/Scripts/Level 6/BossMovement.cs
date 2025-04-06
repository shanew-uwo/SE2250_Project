using UnityEngine;

public class BossMovement : EnemyMovementAI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.speed = 1.5f;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
