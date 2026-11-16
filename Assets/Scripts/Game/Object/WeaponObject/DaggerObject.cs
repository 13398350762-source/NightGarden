using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerObject : BaseWeaponObject
{
    // 刺出时间
    private float attackDuration;

    // 收回时间
    private float returnDuration;
    private bool isAttacking = false;
    private bool isThrusting = false;
    private Vector3 dir;
    private HashSet<BaseEnemyObject> hitEnemies = new HashSet<BaseEnemyObject>();
    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckHit(other);
    }

    private void CheckHit(Collider2D other)
    {
        if (!isThrusting)
            return;

        BaseEnemyObject enemy = other.GetComponentInParent<BaseEnemyObject>();
        if (enemy == null || !hitEnemies.Add(enemy))
            return;

        enemy.GetDamage(atk);
        enemy.GetRepel(repel, dir);
    }

    public override void Attack()
    {
        if (isAttacking || nearestEnemy == null)
            return;

        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        attackDuration = internalTime * 0.2f;
        returnDuration = internalTime * 0.2f;

        hitEnemies.Clear();
        isAttacking = true;
        canAttack = false;

        Vector3 startPos = restLocalPosition;
        transform.localPosition = startPos;

        Vector3 targetPos = transform.parent != null? transform.parent.InverseTransformPoint(nearestEnemy.transform.position): nearestEnemy.transform.position;
        targetPos.z = startPos.z;
        dir = ((Vector2)(nearestEnemy.transform.position - transform.position)).normalized;
        float timer = 0;

        isThrusting = true;
        // 第一阶段：刺出
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / attackDuration);

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }
        isThrusting = false;
        // 第二阶段：收回
        timer = 0;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / returnDuration);

            transform.localPosition = Vector3.Lerp(targetPos, startPos, t);

            yield return null;
        }

        transform.localPosition = startPos;

        isAttacking = false;
    }

    private void Update()
    {
        nearestEnemy = GetNearestEnemy();

        if (!isAttacking)
        {
            if (nearestEnemy != null)
            {
                Vector3 direction = nearestEnemy.transform.position - transform.position;

                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                transform.localEulerAngles = Vector3.zero;
            }
        }

        if (canAttack)
        {
            if (nearestEnemy != null)
                Attack();
        }
        else
        {
            duringTime += Time.deltaTime;

            if (duringTime >= internalTime)
            {
                duringTime = 0;
                canAttack = true;
            }
        }
    }
}
