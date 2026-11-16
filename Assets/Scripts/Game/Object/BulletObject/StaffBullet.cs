using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class StaffBullet : BaseBullet
{
    private float remainTime;
    private SpriteRenderer imgArrow;
    private int loadVersion;
    private BaseEnemyObject enemy=null;
    private bool isHit = false;
    private float infectionTime;
    private Vector2 pos;
    private int enemyVersion;
    private Collider2D bulletCollider;
    private HashSet<BaseEnemyObject> infectedEnemies = new HashSet<BaseEnemyObject>();

    public override void Init(int damage, Vector2 dir, string resName, int repel)
    {
        infectionTime = 6f;
        isHit = false;
        enemy = null;
        infectedEnemies.Clear();
        this.damage = damage;
        this.repel = repel;
        this.dir = dir.normalized;
        remainTime = 3f;
        if (imgArrow == null)
            imgArrow = GetComponent<SpriteRenderer>();
        if (bulletCollider == null)
            bulletCollider = GetComponent<Collider2D>();
        if (bulletCollider != null)
            bulletCollider.enabled = true;
        imgArrow.sortingLayerName = "Bullet";
        imgArrow.enabled = false;
        float angle = Mathf.Atan2(this.dir.y, this.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        int version = ++loadVersion;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Weapon", (atlas) => {
            if (this == null || version != loadVersion || !gameObject.activeInHierarchy)
                return;
            imgArrow.sprite = atlas.GetSprite(resName);
            imgArrow.enabled = !isHit;
        });
    }

    protected override void Update()
    {
        if (isHit)
        {
            infectionTime -= Time.deltaTime;
            if (infectionTime <= 0f)
            {
                Recycle();
                return;
            }
            if (enemy == null || enemy.IsDead() || !enemy.gameObject.activeInHierarchy || enemy.LifeVersion != enemyVersion)
            {
                BaseEnemyObject next = GetNextEnemy();
                if (next == null)
                {
                    Recycle();
                    return;
                }
                Attach(next);
            }
            else
            {
                pos=enemy.transform.position;
                transform.position = enemy.transform.position;
            }
        }
        else
        {
            transform.position += (Vector3)dir * Time.deltaTime * speed;
            remainTime -= Time.deltaTime;
            if (remainTime <= 0f)
                Recycle();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHit)
            return;

        BaseEnemyObject target = collision.GetComponentInParent<BaseEnemyObject>();
        if (target != null && !target.IsDead() && target.gameObject.activeInHierarchy)
        {
            isHit = true;
            imgArrow.enabled = false;
            if (bulletCollider != null)
                bulletCollider.enabled = false;
            target.GetRepel(repel, dir);
            Attach(target);
        }
    }

    private void Attach(BaseEnemyObject target)
    {
        enemy = target;
        enemyVersion = target.LifeVersion;
        infectedEnemies.Add(target);
        pos = target.transform.position;
        transform.position = target.transform.position;
        target.AddEff("magic", infectionTime);
        target.Bleed(damage, infectionTime, 0.4f);
    }

    private BaseEnemyObject GetNextEnemy()
    {
        Collider2D[] collider2ds = Physics2D.OverlapCircleAll(pos, 3f);
        float minDistance = float.MaxValue;
        BaseEnemyObject result = null;
        foreach (var item in collider2ds)
        {
            BaseEnemyObject target = item.GetComponentInParent<BaseEnemyObject>();
            if (target == null || target.IsDead() || !target.gameObject.activeInHierarchy || infectedEnemies.Contains(target))
                continue;

            float distance = Vector2.Distance(target.transform.position, pos);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = target;
            }
        }
        return result;
    }

    private void Recycle()
    {
        enemy = null;
        isHit = false;
        if (imgArrow != null)
            imgArrow.enabled = false;
        if (bulletCollider != null)
            bulletCollider.enabled = false;
        PoolManager.Instance.PushObj("StaffBullet", gameObject);
    }

    private void OnDisable()
    {
        loadVersion++;
        enemy = null;
        isHit = false;
        infectedEnemies.Clear();
    }
}
