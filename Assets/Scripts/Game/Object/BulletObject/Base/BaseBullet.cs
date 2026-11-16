using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class BaseBullet : MonoBehaviour
{
    protected int damage;
    protected Vector2 dir;
    public float speed;
    private float lastTime = 3f;
    protected int repel;
    private SpriteRenderer imgBullet;
    virtual public void Init(int damage,Vector2 dir,string resName,int repel)
    {
        this.damage = damage;
        this.dir = dir;
        this.repel = repel;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) =>{
            atlas.GetSprite(resName);
        });
    }

    virtual protected void Update()
    {
        if (dir != null)
        {
            this.transform.Translate(dir * Time.deltaTime * speed);
        }
        lastTime -= Time.deltaTime;
        if (lastTime <= 0)
            PoolManager.Instance.PushObj(gameObject.name,this.gameObject);
    }
}
