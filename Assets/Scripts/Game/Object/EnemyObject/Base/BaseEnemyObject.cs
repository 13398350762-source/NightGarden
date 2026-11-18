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
    protected float initSpeed;
    protected float speed;
    protected int damage;
    protected int exp;
    private int money;
    public EnemyData enemyData;
    public bool isBoss;
    public Image imgEnemy;
    public Rigidbody2D rb;
    private SpriteRenderer renderEnemy;
    private Animator animator;
    private Sprite spawnSprite;
    private Color spawnColor;
    protected bool isDead=false;
    private Coroutine bleedCoroutine;
    private Coroutine slowSpeedCoroutine;
    private Dictionary<string, GameObject> effects = new Dictionary<string, GameObject>();
    private HashSet<string> loadingEffects = new HashSet<string>();
    private int effectVersion;
    private string poolName;
    protected int maxHp;
    private bool restoreSimulation;
    private int spawnWave = 1;
    public int LifeVersion => effectVersion;
    protected virtual void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        poolName = gameObject.name.Replace("(Clone)", "").Trim();

        Transform visual = transform.Find("Visual");
        if (visual != null)
        {
            animator = visual.GetComponent<Animator>();
            // 回池时恢复动画初始值，避免下次移动继承死亡末帧的透明度和缩放。
            if (animator != null) animator.writeDefaultValuesOnDisable = true;
            renderEnemy = visual.GetComponent<SpriteRenderer>();
            if (renderEnemy != null)
            {
                spawnSprite = renderEnemy.sprite;
                spawnColor = renderEnemy.color;
            }
        }
    }

    protected virtual void OnEnable()
    {
        Init(spawnWave);
    }

    public void Spawn(Vector2 position, int wave, bool boss)
    {
        transform.position = position;
        spawnWave = wave;
        isBoss = boss;
        gameObject.SetActive(true);
    }

    private void LateUpdate()
    {
        if (isDead || rb == null || animator == null || renderEnemy == null)
            return;

        Vector2 velocity = rb.velocity;
        animator.SetBool("IsMoving", velocity.sqrMagnitude > 0.0001f);
        FaceDirection(velocity);
    }

    private void FaceDirection(Vector2 direction)
    {
        if (renderEnemy != null && Mathf.Abs(direction.x) > 0.01f)
            renderEnemy.flipX = direction.x < 0f;
    }

    protected void PlayAttackAnimation(Vector2 direction, string state = "Attack")
    {
        if (isDead) return;
        FaceDirection(direction);
        PlayAnimation(state);
    }

    private bool PlayAnimation(string state)
    {
        if (animator == null || !animator.enabled || !animator.gameObject.activeInHierarchy)
            return false;

        int stateHash = Animator.StringToHash("Base Layer." + state);
        if (!animator.HasState(0, stateHash))
            return false;

        animator.Play(stateHash, 0, 0f);
        return true;
    }

    public virtual void Init(int wave)
    {
        StopAllCoroutines();
        bleedCoroutine = null;
        slowSpeedCoroutine = null;
        effectVersion++;
        isDead = false;
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            if (restoreSimulation) rb.simulated = true;
            rb.velocity = Vector2.zero;
        }
        restoreSimulation = false;
        if (renderEnemy != null)
        {
            renderEnemy.sprite = spawnSprite;
            renderEnemy.color = spawnColor;
            renderEnemy.flipX = false;
        }
        if (animator != null) animator.SetBool("IsMoving", false);
        if (PlayAnimation("Idle")) animator.Update(0f);
        if (enemyData != null)
        {
            enemyID = enemyData.enemyID;
            hp = RandomManager.Instance.GetEnemyHp(enemyData.hp, wave, isBoss);
            speed = enemyData.speed * 1.06f;
            initSpeed = speed;
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

    public void AttackPlayer(Collider2D other)
    {
        if (isDead) return;
        if (PlayerController.IsPlayerBody(other) && PlayerInfoManager.Instance.DecreaseHp(damage, this))
            OnContactHit();
    }

    protected virtual void OnContactHit() { }

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
        if (isDead || !gameObject.activeInHierarchy || damage <= 0)
            return;
        PlayerInfoManager player = PlayerInfoManager.Instance;
        if (GameManager.Instance.IsRunning && player.nowHp > 0 && player.nowHp < player.GetHp() &&
            UnityEngine.Random.value < player.GetDrain())
            player.Heal(1);
        hp-=damage;
        if (DamageNumber.Enabled)
        {
            Collider2D body = GetComponent<Collider2D>();
            Vector3 position = transform.position + Vector3.up;
            if (body != null)
                position = new Vector3(body.bounds.center.x, body.bounds.max.y + 0.25f, transform.position.z);
            DamageNumber.Show(damage, position);
        }
        if (hp <= 0)
        {
            isDead = true;
            Dead();
        }
        else
        {
            AudioManager.Instance.PlaySound(SoundCue.EnemyHit, 0.55f, 0.09f);
            PlayAnimation("Hurt");
        }
    }

    private void Dead()
    {
        StopAllCoroutines();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            restoreSimulation = rb.simulated;
            rb.simulated = false;
        }
        //玩家获得经验
        PlayerInfoManager.Instance.AddExp(exp);
        //爆出金币
        DropMoney(transform.position);
        GameManager.Instance.Death++;
        GameManager.Instance.Score += exp;
        AudioManager.Instance.PlaySound(SoundCue.EnemyDeath, 0.65f, 0.12f);
        if (isBoss && GameManager.Instance.IsRunning && !PlayerInfoManager.Instance.isDead)
            AudioManager.Instance.PlayMusic(MusicTrack.Battle);
        if (PlayAnimation("Death"))
            StartCoroutine(RecycleAfterDeath());
        else
            PoolManager.Instance.PushObj(poolName, gameObject);
    }

    private IEnumerator RecycleAfterDeath()
    {
        animator.Update(0f);
        while (animator != null && animator.enabled && animator.gameObject.activeInHierarchy &&
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;
        PoolManager.Instance.PushObj(poolName, gameObject);
    }

    public void GetRepel(int repel, Vector3 dir)
    {
        if (isDead) return;
        this.transform.position += dir * repel*0.1f;
    }
    public void SlowSpeed(float num, float time)
    {
        if (time <= 0f) return;

        float targetSpeed = initSpeed * Mathf.Clamp01(num);
        if (targetSpeed >= speed) return;

        if (slowSpeedCoroutine != null)
            StopCoroutine(slowSpeedCoroutine);

        slowSpeedCoroutine = StartCoroutine(ReallySlowSpeed(targetSpeed, time));
    }

    private IEnumerator ReallySlowSpeed(float targetSpeed, float time)
    {
        speed = targetSpeed;
        yield return new WaitForSeconds(time);
        speed = initSpeed;
        slowSpeedCoroutine = null;
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
            SpriteRenderer body = renderEnemy != null ? renderEnemy : GetComponent<SpriteRenderer>();
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
    private void DropMoney(Vector3 deathPos)
    {
        SpawnMoney("MoneyObject3", money / 10, deathPos);
        SpawnMoney("MoneyObject2", money % 10 / 5, deathPos);
        SpawnMoney("MoneyObject1", money % 5, deathPos);
    }

    private void SpawnMoney(string resName, int count, Vector3 deathPos)
    {
        if (count <= 0) return;
        GameObject prefab = ResManager.Instance.LoadAsset<GameObject>("money", resName);
        if (prefab == null) return;

        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(prefab, deathPos, prefab.transform.rotation);
            coin.SetActive(true);
            Vector2 dir = Random.insideUnitCircle.normalized;
            float distance = Random.Range(0.5f, 1.5f);
            Vector3 targetPos = deathPos + (Vector3)(dir * distance);
            MonoManager.Instance.StartCoroutine(PopMoney(coin.transform, deathPos, targetPos));
        }
    }
    private IEnumerator PopMoney(Transform money, Vector3 startPos, Vector3 targetPos)
    {
        float timer = 0f;
        float duration = 0.3f;

        while (timer < duration)
        {
            if (money == null || !money.gameObject.activeInHierarchy)
                yield break;

            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            float progress = 1f - (1f - t) * (1f - t);
            money.position = Vector3.Lerp(startPos, targetPos, progress);

            yield return null;
        }

        if (money != null && money.gameObject.activeInHierarchy)
            money.position = targetPos;
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
