using UnityEngine;

public class RangedEnemyHealth : Health
{
    public float enemyMaxHealth = 20f;
    public override void Start()
    {
        base.setMaxHealth(enemyMaxHealth);
        base.Start();
    }

    protected override void Die()
    {
        Destroy(gameObject);
    } 
}
