using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrubEnemy : BaseEnemyObject
{
    public override void Attack()
    {
        
    }

    public override void Move()
    {
        rb.velocity = PlayerDirection() * speed;
    }

    protected void FixedUpdate()
    {
        if (isDead)
            return;
        Move();
    }

}
