using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AccessoryPassive : BaseManager<AccessoryPassive>
{
    public Dictionary<string, UnityAction> passiveDic = new Dictionary<string, UnityAction>();
    private PlayerController silkController;
    private float silkInternalTime = 5f;
    private bool isBuyFirefly;
    private float nextFireflyTime;
    private int thimbleCount;
    private float clockStartTime = -1f;
    private int passiveVersion;
    public bool isBuyChoker=false;
    public bool isChokerTrigger=false;
    public AccessoryPassive()
    {
        passiveDic.Add("蜜晶戒指", Accessory1);
        passiveDic.Add("星露月坠", Accessory2);
        passiveDic.Add("幸运胸针", Accessory3);
        passiveDic.Add("伞语耳坠", Accessory4);
        passiveDic.Add("紫藤晶环", Accessory5);
        passiveDic.Add("丰穗香囊", Accessory6);
        passiveDic.Add("橡心护坠", Accessory7);
        passiveDic.Add("晨露小瓶", Accessory8);
        passiveDic.Add("慢语螺章", Accessory9);
        passiveDic.Add("晴日勋章", Accessory10);
        passiveDic.Add("风叶发夹", Accessory11);
        passiveDic.Add("风铃足链", Accessory12);
        passiveDic.Add("翡翠甲章", Accessory13);
        passiveDic.Add("甜莓心坠", Accessory14);
        passiveDic.Add("蒲风梳", Accessory15);
        passiveDic.Add("蛛绸领结", Accessory16);
        passiveDic.Add("星萤灯坠", Accessory17);
        passiveDic.Add("刻时核表", Accessory18);
        passiveDic.Add("翩羽蜓簪", Accessory19);
        passiveDic.Add("金穗袖扣", Accessory20);
        passiveDic.Add("绯露蔷薇戒", Accessory21);
        passiveDic.Add("秋焰枫扣", Accessory22);
        passiveDic.Add("梦蝶颈环", Accessory23);
        passiveDic.Add("紫莓珠链", Accessory24);
        passiveDic.Add("花匠顶针", Accessory25);
        passiveDic.Add("铃兰耳坠", Accessory26);
    }

    private void Accessory1()
    {
        PlayerInfoManager.Instance.AddHp(3);
    }

    // 星露月坠
    private void Accessory2()
    {
        PlayerInfoManager.Instance.AddAST(2);
        PlayerInfoManager.Instance.AddRange(0.05f);
    }

    // 幸运胸针
    private void Accessory3()
    {
        PlayerInfoManager.Instance.AddLucky(5);
    }

    // 伞语耳坠
    private void Accessory4()
    {
        PlayerInfoManager.Instance.AddAST(1);
        PlayerInfoManager.Instance.AddAtkSpeed(0.08f);
    }

    // 紫藤晶环
    private void Accessory5()
    {
        PlayerInfoManager.Instance.AddAST(3);
        PlayerInfoManager.Instance.AddDamage(0.1f);
        PlayerInfoManager.Instance.AddHp(-2);
    }

    // 丰穗香囊
    private void Accessory6()
    {
        PlayerInfoManager.Instance.AddLucky(8);
        //每回合结束获得8金币
        GameManager.Instance.AddWaveFinishEvent(()=>{
            PlayerInfoManager.Instance.AddMoney(8);
            GameManager.Instance.Collection += 8;
        });
    }

    // 橡心护坠
    private void Accessory7()
    {
        PlayerInfoManager.Instance.AddArmor(1);
        PlayerInfoManager.Instance.AddHp(2);
        PlayerInfoManager.Instance.AddMoveSpeed(-0.03f);
    }

    // 晨露小瓶
    private void Accessory8()
    {
        PlayerInfoManager.Instance.AddHp(4);
    }

    // 慢语螺章
    private void Accessory9()
    {
        PlayerInfoManager.Instance.AddArmor(2);
        PlayerInfoManager.Instance.AddMoveSpeed(-0.05f);
    }

    // 晴日勋章
    private void Accessory10()
    {
        PlayerInfoManager.Instance.AddDamage(0.15f);
        PlayerInfoManager.Instance.AddRecovery(1);
    }

    // 风叶发夹
    private void Accessory11()
    {
        PlayerInfoManager.Instance.AddMoveSpeed(0.05f);
        PlayerInfoManager.Instance.AddRange(0.05f);
    }

    // 风铃足链
    private void Accessory12()
    {
        PlayerInfoManager.Instance.AddAtkSpeed(0.08f);
        PlayerInfoManager.Instance.AddMoveSpeed(0.05f);
    }

    // 翡翠甲章
    private void Accessory13()
    {
        PlayerInfoManager.Instance.AddArmor(3);
        PlayerInfoManager.Instance.AddHp(4);
        PlayerInfoManager.Instance.AddAtkSpeed(-0.05f);
    }

    // 甜莓心坠
    private void Accessory14()
    {
        PlayerInfoManager.Instance.AddHp(4);
    }

    // 蒲风梳
    private void Accessory15()
    {
        PlayerInfoManager.Instance.AddRange(0.15f);
        PlayerInfoManager.Instance.AddSTR(1);
    }

    // 蛛绸领结
    private void Accessory16()
    {
        if (silkController != null)
            return;
        Transform playerPos = PlayerInfoManager.Instance.playerPos;
        if (playerPos == null)
            return;
        silkController = playerPos.GetComponentInParent<PlayerController>();
        if (silkController == null)
            return;
        MonoManager.Instance.AddUpdateEvent(UpdateSilkTime);
        silkController.AddDamageEvent(SilkSlowSpeed);
    }

    private void UpdateSilkTime()
    {
        silkInternalTime = Mathf.Min(5f, silkInternalTime + Time.deltaTime);
    }

    private void SilkSlowSpeed()
    {
        if (silkController == null || silkInternalTime < 5f)
            return;

        silkInternalTime = 0f;
        Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(silkController.transform.position, 5f);
        HashSet<BaseEnemyObject> enemyList = new HashSet<BaseEnemyObject>();
        foreach (var item in collider2Ds)
        {
            BaseEnemyObject enemy = item.GetComponentInParent<BaseEnemyObject>();
            if (enemy == null || enemy.IsDead() || !enemyList.Add(enemy))
                continue;

            enemy.SlowSpeed(0.7f, 3f);
        }
    }

    // 星萤灯坠
    private void Accessory17()
    {
        if (isBuyFirefly)
            return;

        isBuyFirefly = true;
        nextFireflyTime = Time.time + 5f;
        MonoManager.Instance.AddUpdateEvent(FireflyAttack);
    }

    private void FireflyAttack()
    {
        if (!GameManager.Instance.IsRunning || Time.timeScale <= 0f || Time.time < nextFireflyTime)
            return;

        PlayerInfoManager player = PlayerInfoManager.Instance;
        if (player.isDead || player.playerPos == null)
            return;

        Vector2 position = player.playerPos.position;
        BaseEnemyObject nearestEnemy = null;
        float nearestDistance = float.MaxValue;
        foreach (Collider2D item in Physics2D.OverlapCircleAll(position, 6f))
        {
            BaseEnemyObject enemy = item.GetComponentInParent<BaseEnemyObject>();
            if (enemy == null || enemy.IsDead())
                continue;

            float distance = (item.ClosestPoint(position) - position).sqrMagnitude;
            if (distance >= nearestDistance)
                continue;
            nearestDistance = distance;
            nearestEnemy = enemy;
        }

        if (nearestEnemy == null)
            return;

        nextFireflyTime = Time.time + 5f;
        int damage = Mathf.Max(0, Mathf.RoundToInt((player.GetAST() + 4) * player.GetDamage()));
        MonoManager.Instance.StartCoroutine(FlyFirefly(position, nearestEnemy, damage));
    }

    private IEnumerator FlyFirefly(Vector3 position, BaseEnemyObject enemy, int damage)
    {
        int version = passiveVersion;
        int enemyVersion = enemy.LifeVersion;
        GameObject prefab = ResManager.Instance.LoadAsset<GameObject>("eff", "magic");
        GameObject effect = prefab == null ? null : Object.Instantiate(prefab, position, Quaternion.identity);
        float timer = 0f;
        while (version == passiveVersion && GameManager.Instance.IsRunning &&
            enemy != null && !enemy.IsDead() && enemy.LifeVersion == enemyVersion)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / 0.2f);
            if (effect != null)
                effect.transform.position = Vector3.Lerp(position, enemy.transform.position, progress);
            if (progress >= 1f)
            {
                enemy.GetDamage(damage);
                break;
            }
            yield return null;
        }
        if (effect != null)
            Object.Destroy(effect);
    }

    public void Clear()
    {
        passiveVersion++;
        MonoManager.Instance.RemoveUpdateEvent(UpdateSilkTime);
        if (silkController != null)
            silkController.RemoveDamageEvent(SilkSlowSpeed);
        silkController = null;
        silkInternalTime = 5f;

        if (isBuyFirefly)
            MonoManager.Instance.RemoveUpdateEvent(FireflyAttack);
        isBuyFirefly = false;
        nextFireflyTime = 0f;
        thimbleCount = 0;
        clockStartTime = -1f;
        isBuyChoker = false;
        isChokerTrigger = false;
    }

    // 刻时核表
    private void Accessory18()
    {
        PlayerInfoManager.Instance.AddAtkSpeed(0.2f);
        if (clockStartTime < 0f)
            clockStartTime = Time.time;
    }

    public float GetClockAtkSpeed()
    {
        float elapsed = Time.time - clockStartTime;
        return clockStartTime >= 0f && elapsed >= 30f && elapsed % 30f < 4f ? 0.3f : 0f;
    }

    // 翩羽蜓簪
    private void Accessory19()
    {
        PlayerInfoManager.Instance.AddAtkSpeed(0.08f);
        PlayerInfoManager.Instance.AddRange(0.1f);
    }

    // 金穗袖扣
    private void Accessory20()
    {
        PlayerInfoManager.Instance.AddSTR(1);
        PlayerInfoManager.Instance.AddAST(1);
    }

    // 绯露蔷薇戒
    private void Accessory21()
    {
        PlayerInfoManager.Instance.AddDrain(0.03f);
        PlayerInfoManager.Instance.AddDamage(0.1f);
    }

    // 秋焰枫扣
    private void Accessory22()
    {
        PlayerInfoManager.Instance.AddSTR(2);
        PlayerInfoManager.Instance.AddHp(-1);
    }

    // 梦蝶颈环
    private void Accessory23()
    {
        if (isBuyChoker)
            return;
        isBuyChoker = true;
        GameManager.Instance.AddWaveFinishEvent(() => {
            isChokerTrigger = false;
        });
    }

    // 紫莓珠链
    private void Accessory24()
    {
        PlayerInfoManager.Instance.AddHp(3);
        PlayerInfoManager.Instance.AddLucky(3);
    }

    // 花匠顶针
    private void Accessory25()
    {
        PlayerInfoManager.Instance.AddArmor(1);
        thimbleCount++;
    }

    public void ReflectMeleeDamage(BaseEnemyObject attacker)
    {
        if (thimbleCount == 0 || attacker == null || attacker.IsDead())
            return;

        PlayerInfoManager player = PlayerInfoManager.Instance;
        int damage = Mathf.RoundToInt((player.GetSTR() + 3) * player.GetDamage());
        if (damage > 0)
            attacker.GetDamage(damage * thimbleCount);
    }

    // 铃兰耳坠
    private void Accessory26()
    {
        PlayerInfoManager.Instance.AddRecovery(1);
        PlayerInfoManager.Instance.AddAST(1);
    }
}
