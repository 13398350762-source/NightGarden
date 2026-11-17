using UnityEngine;

public class PuffballEnemy : MothEnemy
{
    public override void Move()
    {
        Transform player = PlayerInfoManager.Instance.playerPos;
        if (player == null || isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 offset = (Vector2)player.position - rb.position;
        if (offset.magnitude < keepDistance)
            rb.velocity = -offset.normalized * speed;
        else if (offset.magnitude > keepDistance + 3f)
            rb.velocity = offset.normalized * speed;
        else
            rb.velocity = dir * speed;
    }
}
