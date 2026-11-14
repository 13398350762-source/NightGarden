using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : BaseManager<PlayerInfoManager>
{
    public  List<AccessoryData> accessoryList = new List<AccessoryData>();
    private RoleData role=null;
    public int armor;
    //初始生命值
    public int hp;
    //移动速度
    public float moveSpeed;
    //攻击范围
    public float range;
    //幸运值
    public int lucky;
    //物理伤害
    public int STR;
    //魔法伤害
    public int AST;
    //吸血概率（上限70%）
    public float drain;
    //生命恢复（每秒恢复多少hp，具体回血效果是1/recovery秒回1滴血）
    public int recovery;
    //攻击速度
    public float atkSpeed;
    //初始金币
    public int money;
    //百分比加伤
    public float damage;
    private int level=0;
    private int nextLevelExp;
    private int nowExp;
    public WeaponData[] weaponList = new WeaponData[6];

    public PlayerInfoManager()
    {

    }
    public void SetRole(RoleData data)
    {
        role = data;
        armor = role.armor;
        hp = role.hp;
        moveSpeed = 1;
        range = 1;
        lucky = role.lucky;
        STR = role.STR;
        AST=role.AST;
        drain = role.drain;
        recovery = role.recovery;
        atkSpeed = 1;
        money = role.money;
        damage = role.damage;
    }
    public RoleData GetRole()
    {
        return role;
    }
    public bool SetWeapon(WeaponData data)
    {
        for (int i = 0; i < 6; i++)
        {
            if (weaponList[i] == null)
            {
                weaponList[i] = data;
                return true;
            }
        }
        return false;
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

    public void AddMoveSpeed(float moveSpeed)
    {
        this.moveSpeed += moveSpeed;
    }

    public void AddRange(float range)
    {
        this.range += range;
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
        this.drain += drain;
        if (drain > 0.7)
            drain = 0.7f;
    }

    public void AddRecovery(int recovery)
    {
        this.recovery += recovery;
    }

    public void AddAtkSpeed(float atkSpeed)
    {
        this.atkSpeed += atkSpeed;
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }

    public void AddDamage(int damage)
    {
        this.damage += damage;
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
        return moveSpeed*role.moveSpeed;
    }

    public float GetRange()
    {
        return range*role.range;
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
        return atkSpeed * role.atkSpeed;
    }

    public int GetMoney()
    {
        return money;
    }

    public float GetDamage()
    {
        return damage;
    }
}
