using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BowBullet : BaseBullet
{
    [System.NonSerialized]
    private float remainTime;
    private SpriteRenderer imgArrow;
    private int loadVersion;
    private HashSet<BaseEnemyObject> hitEnemies = new HashSet<BaseEnemyObject>();

    public override void Init(int damage, Vector2 dir, string resName, int repel)
    {
        this.damage = damage;
        this.repel = repel;
        this.dir = dir.normalized;
        remainTime = 3f;
        hitEnemies.Clear();
        if (imgArrow == null)
            imgArrow = GetComponent<SpriteRenderer>();
        imgArrow.sortingLayerName = "Bullet";
        imgArrow.enabled = false;
        float angle = Mathf.Atan2(this.dir.y, this.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        int version = ++loadVersion;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Weapon", (atlas) => {
            if (this == null || version != loadVersion || !gameObject.activeInHierarchy)
                return;
            imgArrow.sprite = atlas.GetSprite(resName);
            imgArrow.enabled = true;
        });
    }

    protected override void Update()
    {
        transform.position += (Vector3)dir * Time.deltaTime * speed;
        remainTime -= Time.deltaTime;
        if (remainTime <= 0f)
            PoolManager.Instance.PushObj("BowBullet", gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BaseEnemyObject enemy = collision.GetComponentInParent<BaseEnemyObject>();
        if (enemy != null && hitEnemies.Add(enemy))
        {
            enemy.GetRepel(repel, dir);
            enemy.GetDamage(damage);
        }
    }

    private void OnDisable()
    {
        loadVersion++;
    }
}
