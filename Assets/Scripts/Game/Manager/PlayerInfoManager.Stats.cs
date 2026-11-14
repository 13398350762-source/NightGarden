using UnityEngine;

// 玩家属性：与 PlayerInfoManager.cs 合并为同一个类。
public partial class PlayerInfoManager
{
    private RoleData role=null;
    // 固定数值通过 Get / Add 方法读写，避免外部绕过属性规则。
    private int armor;
    private int hp;
    private int lucky;
    private int STR;
    private int AST;
    private int recovery; // 每秒恢复的生命值。
    private int money;
    private float drain; // 概率：0.03 = 3%，上限70%。

    private float baseMoveSpeed;
    // 倍率：1 = 无加成，1.2 = +20%；Add 方法传入0.2表示增加20个百分点。
    private float moveSpeedMultiplier = 1f;
    private float rangeMultiplier = 1f;
    private float atkSpeedMultiplier = 1f;
    private float damageMultiplier = 1f;
    private int level=0;
    private int nextLevelExp;
    private int nowExp;

    public void SetRole(RoleData data)
    {
        role = data;
        armor = data?.armor ?? 0;
        hp = data?.hp ?? 0;
        lucky = data?.lucky ?? 0;
        STR = data?.STR ?? 0;
        AST = data?.AST ?? 0;
        recovery = data?.recovery ?? 0;
        money = data?.money ?? 0;
        drain = Mathf.Clamp(data?.drain ?? 0f, 0f, 0.7f);
        baseMoveSpeed = Mathf.Max(0f, data?.moveSpeed ?? 0f);
        moveSpeedMultiplier = 1f;
        rangeMultiplier = Mathf.Max(0f, data?.range ?? 1f);
        atkSpeedMultiplier = Mathf.Max(0f, data?.atkSpeed ?? 1f);
        damageMultiplier = Mathf.Max(0f, data?.damage ?? 1f);
    }
    public RoleData GetRole()
    {
        return role;
    }
    public void AddAccessory(AccessoryData data)
    {
        accessoryList.Add(data);
    }
    public void AddArmor(int armor)
    {
        this.armor += armor;
    }

    public void AddHp(int hp)
    {
        this.hp += hp;
    }

    public void AddMoveSpeed(float bonus)
    {
        moveSpeedMultiplier = Mathf.Max(0f, moveSpeedMultiplier + bonus);
    }

    public void AddRange(float bonus)
    {
        rangeMultiplier = Mathf.Max(0f, rangeMultiplier + bonus);
    }

    public void AddLucky(int lucky)
    {
        this.lucky += lucky;
    }

    public void AddSTR(int STR)
    {
        this.STR += STR;
    }

    public void AddAST(int AST)
    {
        this.AST += AST;
    }

    public void AddDrain(float drain)
    {
        this.drain = Mathf.Clamp(this.drain + drain, 0f, 0.7f);
    }

    public void AddRecovery(int recovery)
    {
        this.recovery += recovery;
    }

    public void AddAtkSpeed(float bonus)
    {
        atkSpeedMultiplier = Mathf.Max(0f, atkSpeedMultiplier + bonus);
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }

    public void AddDamage(float bonus)
    {
        damageMultiplier = Mathf.Max(0f, damageMultiplier + bonus);
    }

    public int GetArmor()
    {
        return armor;
    }

    public int GetHp()
    {
        return hp;
    }

    public float GetMoveSpeed()
    {
        return baseMoveSpeed * moveSpeedMultiplier;
    }

    // 面板使用倍率显示百分比；移动逻辑使用上面的最终速度。
    public float GetMoveSpeedMultiplier()
    {
        return moveSpeedMultiplier;
    }

    // 以下返回最终倍率，已包含角色初始加成，使用时不要再乘 RoleData。
    public float GetRange()
    {
        return rangeMultiplier;
    }

    public int GetLucky()
    {
        return lucky;
    }

    public int GetSTR()
    {
        return STR;
    }

    public int GetAST()
    {
        return AST;
    }

    public float GetDrain()
    {
        return drain;
    }

    public int GetRecovery()
    {
        return recovery;
    }

    public float GetAtkSpeed()
    {
        return atkSpeedMultiplier;
    }

    public int GetMoney()
    {
        return money;
    }

    public float GetDamage()
    {
        return damageMultiplier;
    }
}
