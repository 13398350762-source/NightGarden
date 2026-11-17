using UnityEngine;

public class SlimeEnemy : MothEnemy
{
    public override void Move()
    {
        if (isMoving)
            base.Move();
        else
            rb.velocity = Vector2.zero;
    }
}
