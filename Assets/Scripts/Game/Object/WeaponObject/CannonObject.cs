using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonObject : BaseWeaponObject
{
    public Transform muzzle;

    public override void Init(WeaponData data)
    {
        base.Init(data);
        if (muzzle == null)
            muzzle = transform.Find("Muzzle");
        if (muzzle == null)
        {
            muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(transform, false);
            muzzle.localPosition = new Vector3(1.36f, 0.54f, 0f);
            muzzle.localEulerAngles = new Vector3(0f, 0f, 35f);
        }
    }

    public override void Attack()
    {
        if (!canAttack || nearestEnemy == null)
            return;

        if (muzzle == null)
            return;

        AimAtEnemy();
        canAttack = false;
        duringTime = 0f;
        Vector2 dir = muzzle.right;

        Vector3 position = muzzle.position;
        int damage = atk;
        int knockback = repel;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        PoolManager.Instance.GetObj("CannonBullet", "bullet", (obj) =>
        {
            if (obj == null)
                return;
            Transform trans = (obj as GameObject).transform;
            trans.position = position;
            trans.rotation = Quaternion.Euler(0f, 0f, angle);
            CannonBullet bullet = trans.GetComponent<CannonBullet>();
            if (bullet == null)
            {
                Debug.LogError("CannonBullet 预制体没有挂 CannonBullet 脚本", trans);
                return;
            }
            bullet.Init(damage, dir, "projectile_03", knockback);
        });
    }

    private void Update()
    {
        nearestEnemy = GetNearestEnemy();

        if (nearestEnemy != null)
        {
            AimAtEnemy();
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

    private void AimAtEnemy()
    {
        if (muzzle == null || nearestEnemy == null)
            return;

        Collider2D enemyCollider = nearestEnemy.GetComponent<Collider2D>();
        Vector3 targetPos = enemyCollider != null ? enemyCollider.bounds.center : nearestEnemy.transform.position;
        Vector2 direction = targetPos - transform.position;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        // 花洒口偏离水壶中心，瞄准时补偿这段偏移。
        Vector2 muzzleOffset = muzzle.position - transform.position;
        Vector2 muzzleDirection = muzzle.right;
        float sideOffset = muzzleOffset.x * muzzleDirection.y - muzzleOffset.y * muzzleDirection.x;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += Mathf.Asin(Mathf.Clamp(sideOffset / direction.magnitude, -1f, 1f)) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - muzzle.localEulerAngles.z);
    }
}
