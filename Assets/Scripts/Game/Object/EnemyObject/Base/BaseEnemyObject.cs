using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public abstract class BaseEnemyObject : MonoBehaviour
{
    protected string enemyID;
    public int hp;
    protected float speed;
    protected int damage;
    protected int exp;
    public EnemyData enemyData;
    public Image imgEnemy;
    private bool isDead=false;
    public virtual void Init(int wave)
    {
        isDead = false;
        enemyID = enemyData.enemyID;
        speed = enemyData.speed;
        //根据波数和基础数据获得hp,damage,exp;
        //加载敌人图片资源
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "", (atlas) => { 
        
        });
    }

    public void GetDamage(int damage)
    {
        hp-=damage;
        //触发受伤动画
        if (hp <= 0)
            isDead=true;
        if (isDead)
            Dead();
    }
    private void Dead()
    {
        //触发死亡动画
        //玩家获得经验
        //爆出金币
        PoolManager.Instance.PushObj(enemyID, this.gameObject);
    }
    public void GetRepel(int repel, Vector3 dir)
    {
        this.transform.position += dir * repel*0.1f;
    }
    public void SlowSpeed(float num)
    {
        speed *= num;
    }
    public abstract void Attack();
    public abstract void Move();
}
