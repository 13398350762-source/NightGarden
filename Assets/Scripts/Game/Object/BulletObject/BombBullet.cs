using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BombBullet : BaseBullet
{
    private float remainTime;
    private SpriteRenderer imgArrow;
    private int loadVersion;
    private bool isExploded;

    public override void Init(int damage, Vector2 dir, string resName, int repel)
    {
        this.damage = damage;
        this.repel = repel;
        this.dir = dir.normalized;
        remainTime = 3f;
        isExploded = false;
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
            PoolManager.Instance.PushObj("BombBullet", gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isExploded)
            return;

        BaseEnemyObject enemy = collision.GetComponentInParent<BaseEnemyObject>();
        if (enemy != null)
        {
            isExploded = true;
            Vector3 pos = enemy.transform.position;
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(pos, 2);
            HashSet<BaseEnemyObject> hitEnemies = new HashSet<BaseEnemyObject>();
            foreach (var item in collider2Ds)
            {
                BaseEnemyObject target = item.GetComponentInParent<BaseEnemyObject>();
                if (target != null)
                    hitEnemies.Add(target);
            }
            foreach (var target in hitEnemies)
            {
                target.GetRepel(repel, dir);
                target.GetDamage(damage);
            }
            //产生爆炸效果
            ResManager.Instance.LoadResAsync<GameObject>("eff", "bomb_explosion", (obj) =>{
                if (obj == null) return;
                obj.transform.position = pos;
                ResManager.Instance.StartCoroutine(RecycleExplosion(obj));
            });
            Destroy(gameObject);
        }
    }

    private void OnDisable()
    {
        loadVersion++;
    }
    private static IEnumerator RecycleExplosion(GameObject obj)
    {
        yield return new WaitForSeconds(2f);

        if (obj != null)
            PoolManager.Instance.PushObj("bomb_explosion", obj);
    }
}
