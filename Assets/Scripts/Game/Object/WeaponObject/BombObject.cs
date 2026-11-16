using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombObject : BaseWeaponObject
{
    public override void Attack()
    {
        if (!canAttack || nearestEnemy == null)
            return;

        canAttack = false;
        duringTime = 0f;
        Collider2D enemyCollider = nearestEnemy.GetComponent<Collider2D>();
        Vector3 targetPos = enemyCollider != null ? enemyCollider.bounds.center : nearestEnemy.transform.position;
        Vector2 dir = ((Vector2)(targetPos - transform.position)).normalized;

        Vector3 position = transform.position;
        int damage = atk;
        int knockback = repel;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        PoolManager.Instance.GetObj("BombBullet", "bullet", (obj) =>
        {
            if (obj == null)
                return;
            Transform trans = (obj as GameObject).transform;
            trans.position = position;
            trans.rotation = Quaternion.Euler(0f, 0f, angle);
            trans.GetComponent<BombBullet>().Init(damage, dir, "projectile_08", knockback);
        });
    }

    private void Update()
    {
        nearestEnemy = GetNearestEnemy();

        if (nearestEnemy != null)
        {
            Collider2D enemyCollider = nearestEnemy.GetComponent<Collider2D>();
            Vector3 targetPos = enemyCollider != null ? enemyCollider.bounds.center : nearestEnemy.transform.position;
            Vector2 dir = targetPos - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else
        {
            transform.localEulerAngles = Vector3.zero;
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
