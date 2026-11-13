using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType 
{
    STR,
    AST
}

public class WeaponData:ScriptableObject
{
    //武器ID
    public string weaponID;
    //武器名字
    public string Name;
    //武器词条
    public string Tip;
    //伤害类型
    public DamageType damageType;
    //武器类型
    public string Mode;
    //特效资源
    public string resEff;
    //资源路径
    public string resName;
    //子弹ID
    public string bullet;
    //武器等级
    public int level;
    //攻击间隔
    public float internalTime;
    //武器范围
    public int range;
    //武器伤害
    public int atk;
    //击退效果
    public int repel;
}
