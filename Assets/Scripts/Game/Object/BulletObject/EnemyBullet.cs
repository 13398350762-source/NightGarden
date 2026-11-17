using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class EnemyBullet : BaseBullet
{
    private float remainTime;
    private SpriteRenderer imgArrow;
    private int loadVersion;
    private bool isRecycled;

    public override void Init(int damage, Vector2 dir, string resName = "enemy_orb_spore", int repel=0)
    {
        this.damage = damage;
        this.repel = repel;
        this.dir = dir.normalized;
        remainTime = 5f;
        isRecycled = false;
        if (imgArrow == null)
            imgArrow = GetComponent<SpriteRenderer>();
        imgArrow.sortingLayerName = "Bullet";
        imgArrow.enabled = false;
        float angle = Mathf.Atan2(this.dir.y, this.dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        int version = ++loadVersion;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "enemy", (atlas) => {
            if (this == null || version != loadVersion || !gameObject.activeInHierarchy)
                return;
            if (atlas == null) return;
            Sprite sprite = atlas.GetSprite(resName);
            if (sprite != null) imgArrow.sprite = sprite;
            imgArrow.enabled = imgArrow.sprite != null;
        });
    }

    protected override void Update()
    {
        if (isRecycled) return;
        transform.position += (Vector3)dir * Time.deltaTime * speed;
        remainTime -= Time.deltaTime;
        if (remainTime <= 0f)
            Recycle();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRecycled) return;
        if (PlayerController.IsPlayerBody(collision))
        {
            PlayerInfoManager.Instance.DecreaseHp(damage);
            Recycle();
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Wall")) Recycle();
    }

    private void Recycle()
    {
        if (isRecycled) return;
        isRecycled = true;
        PoolManager.Instance.PushObj("EnemyBullet", gameObject);
    }

    private void OnDisable()
    {
        loadVersion++;
        isRecycled = true;
    }
}
