using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleData
{
    public string roleID;
    //初始护甲
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
}
