using UnityEngine;
using System.Collections;

public class MothEnemy : BaseEnemyObject
{
    public float internalTime = 2.4f;
    public float switchTime = 1.5f;
    public int bulletCount = 3;
    public float spread = 24f;
    public float keepDistance = 8f;
    public float warningTime = 0.3f;
    private float attackTimer;
    private float moveTimer;
    protected bool isAttacking;
    protected bool isMoving;
    protected int shotCount;
    protected Vector2 dir;

    public override void Init(int wave)
    {
        base.Init(wave);
        attackTimer = 0f;
        moveTimer = 0f;
        isAttacking = false;
        isMoving = true;
        shotCount = 0;
        SwitchDir();
    }

    public override void Attack()
    {
        if (isDead || isAttacking || PlayerInfoManager.Instance.playerPos == null) return;
        isAttacking = true;
        StartCoroutine(ReallyAttack());
    }

    private IEnumerator ReallyAttack()
    {
        rb.velocity = Vector2.zero;
        PlayAttackAnimation(PlayerDirection());
        yield return new WaitForSeconds(warningTime);
        if (!isDead && PlayerInfoManager.Instance.playerPos != null)
        {
            Shoot();
            shotCount++;
        }
        isAttacking = false;
    }

    protected virtual void Shoot()
    {
        FireFan(bulletCount, spread, PlayerDirection());
    }

    public override void Move()
    {
        Transform player = PlayerInfoManager.Instance.playerPos;
        if (player == null || isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        Vector2 toward = (Vector2)player.position - rb.position;
        if (toward.magnitude > keepDistance * 2f)
            rb.velocity = toward.normalized * speed;
        else
            rb.velocity = dir * speed;
    }

    protected void FixedUpdate()
    {
        if (isDead) return;
        Move();
        attackTimer += Time.fixedDeltaTime;
        if (attackTimer >= internalTime && !isAttacking && PlayerInfoManager.Instance.playerPos != null)
        {
            Attack();
            attackTimer = 0f;
        }
        moveTimer += Time.fixedDeltaTime;
        if (moveTimer >= switchTime)
        {
            SwitchDir();
            isMoving = !isMoving;
            moveTimer = 0f;
        }
    }

    private void SwitchDir()
    {
        dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.right;
    }
}
