using UnityEngine;

public class RotrootKingEnemy : MothEnemy
{
    public override void Move()
    {
        rb.velocity = isAttacking ? Vector2.zero : PlayerDirection() * speed;
    }

    protected override void Shoot()
    {
        FireRing(12);
        if (hp <= maxHp / 2)
            FireFan(5, 48f, PlayerDirection());
    }
}
