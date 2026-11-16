using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChiliGunObject : BaseWeaponObject
{
    public Transform muzzle;
    public float flameWidth = 1.6f;
    public HashSet<BaseEnemyObject> hitEnemies = new HashSet<BaseEnemyObject>();
    private ChiliFlame flame;
    private bool isAttacking;
    private Vector2 attackDir;
    private float attackRange;
    private float attackWidth;
    private float remainTime;
    private int loadVersion;

    public override void Init(WeaponData data)
    {
        base.Init(data);
        if (muzzle == null)
            muzzle = transform.Find("Muzzle");
        if (muzzle == null)
        {
            muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(transform, false);
            muzzle.localPosition = new Vector3(1.4f, 0.15f, 0f);
        }
    }

    public override void Attack()
    {
        if (!canAttack || isAttacking || nearestEnemy == null || muzzle == null || range <= 0f)
            return;

        canAttack = false;
        isAttacking = true;
        duringTime = 0f;
        hitEnemies.Clear();
        Collider2D enemyCollider = nearestEnemy.GetComponent<Collider2D>();
        Vector3 targetPos = enemyCollider != null ? enemyCollider.bounds.center : nearestEnemy.transform.position;
        attackDir = ((Vector2)(targetPos - muzzle.position)).normalized;
        if (attackDir.sqrMagnitude < 0.0001f)
            attackDir = transform.right;
        attackRange = range;
        attackWidth = Mathf.Max(0.05f, flameWidth);
        int version = ++loadVersion;

        PoolManager.Instance.GetObj("ChiliFlame", "eff", (obj) =>
        {
            if (obj == null)
            {
                if (this != null && version == loadVersion)
                {
                    isAttacking = false;
                    canAttack = true;
                }
                return;
            }
            GameObject effect = obj as GameObject;
            if (this == null || version != loadVersion || !isActiveAndEnabled)
            {
                PoolManager.Instance.PushObj("ChiliFlame", effect);
                return;
            }
            flame = effect.GetComponent<ChiliFlame>();
            if (flame == null)
            {
                Debug.LogError("ChiliFlame 预制体没有挂 ChiliFlame 脚本", effect);
                PoolManager.Instance.PushObj("ChiliFlame", effect);
                isAttacking = false;
                canAttack = true;
                return;
            }
            flame.transform.position = muzzle.position;
            flame.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg);
            remainTime = 0.25f;
            flame.Init(attackRange, attackWidth);
            CheckHit();
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
            attackDir = ((Vector2)(targetPos - muzzle.position)).normalized;
            if (attackDir.sqrMagnitude < 0.0001f)
                attackDir = transform.right;
        }
        else if (!isAttacking)
        {
            transform.localEulerAngles = Vector3.zero;
        }

        if (flame != null)
        {
            flame.transform.position = muzzle.position;
            if (nearestEnemy != null && range > 0f)
            {
                flame.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg);
                if (!Mathf.Approximately(attackRange, range) || !Mathf.Approximately(attackWidth, Mathf.Max(0.05f, flameWidth)))
                {
                    attackRange = range;
                    attackWidth = Mathf.Max(0.05f, flameWidth);
                    flame.Init(attackRange, attackWidth);
                }
                flame.Play();
                remainTime = 0.25f;
                duringTime += Time.deltaTime;
                if (duringTime >= 0.1f)
                {
                    duringTime %= 0.1f;
                    CheckHit();
                }
            }
            else
            {
                flame.Stop();
                hitEnemies.Clear();
                remainTime -= Time.deltaTime;
                if (remainTime <= 0f)
                    ReturnFlame();
            }
        }
        else if (isAttacking && nearestEnemy == null)
        {
            ReturnFlame();
        }

        if (canAttack && nearestEnemy != null)
            Attack();
    }

    private void CheckHit()
    {
        hitEnemies.Clear();
        Vector2 center = (Vector2)muzzle.position + attackDir * attackRange * 0.5f;
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(center, new Vector2(attackRange, attackWidth), angle);
        foreach (var item in colliders)
        {
            BaseEnemyObject enemy = item.GetComponentInParent<BaseEnemyObject>();
            if (enemy != null && hitEnemies.Add(enemy))
            {
                if (!enemy.HasEff("burning_embers"))
                {
                    enemy.AddEff("burning_embers", 5f);
                    enemy.Bleed(atk, 5f, Mathf.Max(0.1f, internalTime));
                }
            }
        }
    }

    private void ReturnFlame()
    {
        loadVersion++;
        if (flame != null)
        {
            flame.Clear();
            PoolManager.Instance.PushObj("ChiliFlame", flame.gameObject);
            flame = null;
        }
        isAttacking = false;
        canAttack = true;
        duringTime = 0f;
        hitEnemies.Clear();
    }

    private void OnDisable()
    {
        ReturnFlame();
    }
}
