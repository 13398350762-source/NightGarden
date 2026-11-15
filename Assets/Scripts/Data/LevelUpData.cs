using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpData:ScriptableObject
{
    public string statsName;
    public int level;
    public int num;
    public string resName;

    // 将表格数值转成 Add 方法需要的加成值，不包含基础倍率1。
    public static float GetValue(string statsName, int num)
    {
        switch (statsName)
        {
            case "damage":
            case "atkSpeed":
            case "range":
            case "moveSpeed":
            case "drain":
                return num / 100f;
            case "armor":
            case "hp":
            case "lucky":
            case "STR":
            case "AST":
            case "recovery":
            case "money":
                return num;
            default:
                throw new System.ArgumentException("未知的属性名称：" + statsName, nameof(statsName));
        }
    }
}
