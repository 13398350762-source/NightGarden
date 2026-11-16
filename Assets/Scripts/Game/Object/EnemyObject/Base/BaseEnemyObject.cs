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
    private Coroutine bleedCoroutine;
    private Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();
    private HashSet<string> loadingEffects = new HashSet<string>();
    private int effectVersion;
    public int LifeVersion => effectVersion;
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
        if (isDead || !gameObject.activeInHierarchy)
            return;
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
        PoolManager.Instance.PushObj(string.IsNullOrEmpty(enemyID) ? gameObject.name : enemyID, this.gameObject);
    }
    public void GetRepel(int repel, Vector3 dir)
    {
        this.transform.position += dir * repel*0.1f;
    }
    public void SlowSpeed(float num)
    {
        speed *= num;
    }
    public bool HasEff(string name)
    {
        return loadingEffects.Contains(name) || (effects.TryGetValue(name, out GameObject effect) && effect != null);
    }

    public void AddEff(string name, float lastTime)
    {
        if (isDead || !isActiveAndEnabled || lastTime <= 0f)
            return;
        if (!loadingEffects.Add(name))
            return;

        int version = effectVersion;
        ResManager.Instance.LoadResAsync<GameObject>("eff", name, (obj) =>{
            if (obj == null)
            {
                if (this != null && version == effectVersion)
                    loadingEffects.Remove(name);
                return;
            }
            if (this == null || version != effectVersion || isDead || !isActiveAndEnabled)
            {
                Destroy(obj);
                return;
            }
            loadingEffects.Remove(name);
            if (effects.TryGetValue(name, out GameObject oldEffect) && oldEffect != null)
                Destroy(oldEffect);

            effects[name] = obj;
            obj.name = name;
            obj.transform.SetParent(this.transform, false);
            SpriteRenderer body = GetComponent<SpriteRenderer>();
            if (body != null)
            {
                obj.transform.position = body.bounds.center;
                foreach (var renderer in obj.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    renderer.sortingLayerID = body.sortingLayerID;
                    renderer.sortingOrder = body.sortingOrder + 1;
                }
            }
            Destroy(obj, lastTime);
        });
    }

    private void OnDisable()
    {
        effectVersion++;
        foreach (var effect in effects.Values)
        {
            if (effect != null)
                Destroy(effect);
        }
        effects.Clear();
        loadingEffects.Clear();
    }
    public void Bleed(int damage, float lastTime, float internalTime)
    {
        if (bleedCoroutine != null)
            StopCoroutine(bleedCoroutine);

        bleedCoroutine = null;

        if (isDead || lastTime <= 0f || internalTime <= 0)
            return;

        bleedCoroutine = StartCoroutine(ReallyBleed(damage, lastTime, internalTime));
    }

    private IEnumerator ReallyBleed(int damage, float lastTime, float internalTime)
    {
        float during = 0f;

        while (during < lastTime && !isDead)
        {
            GetDamage(damage);

            if (isDead)
                break;

            yield return new WaitForSeconds(internalTime);
            during += internalTime;
        }

        bleedCoroutine = null;
    }
    public bool IsDead()
    {
        return isDead;
    }
    public abstract void Attack();
    public abstract void Move();
}
