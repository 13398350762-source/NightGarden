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
    private int money;
    public EnemyData enemyData;
    public bool isBoss;
    public Image imgEnemy;
    public Rigidbody2D rb;
    protected bool isDead=false;
    private Coroutine bleedCoroutine;
    private Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();
    private HashSet<string> loadingEffects = new HashSet<string>();
    private int effectVersion;
    private string poolName;
    protected int maxHp;
    public int LifeVersion => effectVersion;
    protected virtual void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        poolName = gameObject.name.Replace("(Clone)", "").Trim();
    }

    protected virtual void OnEnable()
    {
        Init(1);
    }

    public virtual void Init(int wave)
    {
        StopAllCoroutines();
        bleedCoroutine = null;
        effectVersion++;
        isDead = false;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;
        if (enemyData != null)
        {
            enemyID = enemyData.enemyID;
            hp = RandomManager.Instance.GetEnemyHp(enemyData.hp, wave, isBoss);
            speed = enemyData.speed;
            damage = enemyData.damage;
            exp = LevelManager.Instance.GetEnemyExp(enemyData.exp, wave);
            money = RandomManager.Instance.GetEnemyMoney(enemyData.exp, wave, PlayerInfoManager.Instance.GetLucky(), isBoss);
        }
        else
        {
            speed = 5f;
            money = 0;
        }
        maxHp = Mathf.Max(1, hp);
    }

    public int GetMoney()
    {
        return money;
    }

    public void AttackPlayer(Collider2D other)
    {
        if (isDead) return;
        if (PlayerController.IsPlayerBody(other))
            PlayerInfoManager.Instance.DecreaseHp(damage);
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        AttackPlayer(collision.collider);
    }

    protected Vector2 PlayerDirection()
    {
        Transform player = PlayerInfoManager.Instance.playerPos;
        return player == null ? Vector2.zero : ((Vector2)player.position - rb.position).normalized;
    }

    protected void FireFan(int count, float spread, Vector2 direction)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = count == 1 ? 0f : -spread * 0.5f + spread * i / (count - 1);
            FireBullet(Quaternion.Euler(0, 0, angle) * direction);
        }
    }

    protected void FireRing(int count, float offset = 0f)
    {
        for (int i = 0; i < count; i++)
            FireBullet(Quaternion.Euler(0, 0, offset + 360f * i / count) * Vector2.right);
    }

    private void FireBullet(Vector2 direction)
    {
        if (direction.sqrMagnitude < 0.001f || isDead) return;
        Vector3 position = rb.position;
        int attack = damage;
        int version = effectVersion;
        PoolManager.Instance.GetObj("EnemyBullet", "enemy", obj => {
            GameObject bullet = obj as GameObject;
            if (bullet == null) return;
            if (this == null || !isActiveAndEnabled || isDead || version != effectVersion)
            {
                PoolManager.Instance.PushObj("EnemyBullet", bullet);
                return;
            }
            bullet.transform.position = position;
            bullet.SetActive(true);
            bullet.GetComponent<EnemyBullet>().Init(attack, direction);
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
        PlayerInfoManager.Instance.AddExp(exp);
        //爆出金币
        if (rb != null) rb.velocity = Vector2.zero;
        PoolManager.Instance.PushObj(poolName, this.gameObject);
        GameManager.Instance.Death++;
        GameManager.Instance.Score += exp;
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
    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        bleedCoroutine = null;
        if (rb != null) rb.velocity = Vector2.zero;
        effectVersion++;
        foreach (var effect in effects.Values)
        {
            if (effect != null)
                Destroy(effect);
        }
        effects.Clear();
        loadingEffects.Clear();
    }
}
