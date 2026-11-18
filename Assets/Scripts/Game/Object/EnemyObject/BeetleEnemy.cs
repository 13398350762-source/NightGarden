using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleEnemy : BaseEnemyObject
{
    private bool isMove = false;
    private bool isAttacking = false;
    private bool isIdle = false;
    public float distance = 50;
    public float sprint = 28f;
    public float roamRadius = 5f;
    public float roamTime = 2f;
    public float chargeTime = 0.5f;
    public float idleTime = 1f;
    public bool ringAfterCharge;
    public bool doubleChargeAtHalfHp;
    private Vector2 moveTarget;
    private bool hasMoveTarget;
    private float nextMoveTargetTime;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init(int wave)
    {
        base.Init(wave);
        ResetAttack();
    }

    private void ResetAttack()
    {
        StopAllCoroutines();
        isMove = false;
        isAttacking = false;
        isIdle = false;
        hasMoveTarget = false;
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    public override void Attack()
    {
        if (isDead || isAttacking || isMove || PlayerInfoManager.Instance.playerPos == null)
            return;
        isAttacking = true;
        StartCoroutine(ReallyAttack());
    }

    public override void Move()
    {
        Transform player = PlayerInfoManager.Instance.playerPos;

        if (player == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (!hasMoveTarget || Time.time >= nextMoveTargetTime)
        {
            moveTarget = (Vector2)player.position + Random.insideUnitCircle * roamRadius;
            hasMoveTarget = true;
            nextMoveTargetTime = Time.time + 3f;
        }

        Vector2 offset = moveTarget - rb.position;
        if (offset.magnitude <= 0.2f)
        {
            rb.velocity = Vector2.zero;
            hasMoveTarget = false;
            return;
        }
        rb.velocity = offset.normalized * Mathf.Min(speed, offset.magnitude / Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (isIdle)
            return;
        if (isMove)
        {
            Move();
            return;
        }
        else
        {
            if (!isAttacking)
                Attack();
        }
    }
    private IEnumerator ReallyAttack()
    {
        rb.velocity = Vector2.zero;
        PlayAttackAnimation(PlayerDirection(), "Charge");
        yield return new WaitForSeconds(chargeTime);
        Transform player = PlayerInfoManager.Instance.playerPos;
        if (player == null || isDead)
        {
            isAttacking = false;
            yield break;
        }
        int count = doubleChargeAtHalfHp && hp <= maxHp / 2 ? 2 : 1;
        for (int i = 0; i < count; i++)
        {
            if (player == null || isDead) break;
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            PlayAttackAnimation(dir);
            rb.velocity = dir * sprint;
            Vector2 startPos = rb.position;
            float elapsed = 0f;
            float maxDuration = Mathf.Max(0f, distance) / Mathf.Max(sprint, 0.01f);
            while (!isDead && Vector2.Distance(rb.position, startPos) < distance && elapsed < maxDuration)
            {
                yield return new WaitForFixedUpdate();
                elapsed += Time.fixedDeltaTime;
            }
            rb.velocity = Vector2.zero;
            if (ringAfterCharge && !isDead) FireRing(12);
            if (i + 1 < count) yield return new WaitForSeconds(0.3f);
        }
        rb.velocity = Vector2.zero;
        if (isDead)
            yield break;
        isIdle = true;
        //播放待机动画
        yield return new WaitForSeconds(idleTime);
        isMove = true;
        isAttacking = false;
        isIdle = false;
        hasMoveTarget = false;
        //播放移动动画
        yield return new WaitForSeconds(roamTime);
        isMove = false;
        rb.velocity = Vector2.zero;
    }

}
