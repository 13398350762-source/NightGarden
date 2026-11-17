using JetBrains.Annotations;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerInfoManager : BaseManager<PlayerInfoManager>
{
    public  List<AccessoryData> accessoryList = new List<AccessoryData>();
    public WeaponData[] weaponList = new WeaponData[6];
    public WeaponData initialWeapon;
    public int nowHp;
    public int nextLevelExp;
    public int nowExp;
    public Transform playerPos;
    public bool isDead;
    private float nextDamageTime;
    public float invincibleTime = 0.2f;
    public void Init()
    {
        nowExp = 0;
        nextDamageTime = 0f;
        isDead = false;
        level = 0;
        nowHp = hp;
    }
    public void LevelUp()
    {
        while (nowExp >= nextLevelExp)
        {
            nowExp -= nextLevelExp;
            level++;
            LevelUpManager.Instance.selectCount++;
            nextLevelExp = LevelManager.Instance.GetNextLevelExp(level);
        }
    }
    public void SetInitialWeapon(WeaponData data)
    {
        initialWeapon = data;
        SetWeapon(data);
    }
    public bool SetWeapon(WeaponData data)
    {
        if (data == null)
            return false;
        int pos = 0;
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (weaponList[i] != null)
                weaponList[pos++] = weaponList[i];
        }
        for (int i = pos; i < weaponList.Length; i++)
            weaponList[i] = null;
        if (pos == weaponList.Length)
            return false;
        weaponList[pos] = data;
        return true;
    }

    public bool CompositeWeapon(int index)
    {
        if (index < 0 || index >= weaponList.Length || weaponList[index] == null || weaponList[index].level >= 4)
            return false;
        int next = -1;
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (i != index && weaponList[i] != null && weaponList[i].weaponID == weaponList[index].weaponID && weaponList[i].level == weaponList[index].level)
            {
                next = i;
                break;
            }
        }
        if (next == -1)
            return false;
        int nextlevel = weaponList[index].level + 1;
        string id = weaponList[index].weaponID;
        string path = $"ScriptableObjects/WeaponDatas/{id}/{id}_{nextlevel}";
        WeaponData data = Resources.Load<WeaponData>(path);
        if (data == null)
            return false;
        WeaponData[] datas = new WeaponData[weaponList.Length];
        int pos = 0;
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (i != index && i != next && weaponList[i] != null)
            {
                datas[pos++] = weaponList[i];
            }
        }
        datas[pos] = data;
        weaponList = datas;
        return true;
    }

    public void SellWeapon(int index)
    {
        if (index < 0 || index >= weaponList.Length || weaponList[index] == null)
            return;
        WeaponData[] newweaponList = new WeaponData[weaponList.Length];
        int pos = 0;
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (i != index && weaponList[i] != null)
                newweaponList[pos++]= weaponList[i];
        }
        int level= weaponList[index].level;
        weaponList = newweaponList;
        AddMoney(RandomManager.Instance.RandomSellPrice(level, GameManager.Instance.Wave));
    }

    public void DecreaseHp(int damage)
    {
        if (isDead || damage <= 0 || Time.time < nextDamageTime)
            return;

        nextDamageTime = Time.time + invincibleTime;
        nowHp = Mathf.Max(0, nowHp - damage);

        if (nowHp <= 0)
        {
            isDead = true;
            Dead();
            return;
        }
    }

    private void Dead()
    {
        isDead = true;
        //播放死亡动画
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }
    public void AddExp(int exp)
    {
        nowExp += exp;
        LevelUp();
    }

    public void Clear()
    {
        Init();
        System.Array.Clear(weaponList, 0, weaponList.Length);
        accessoryList.Clear();
        SetRole(null);
    }
}
