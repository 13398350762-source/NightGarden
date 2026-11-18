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
    public float invincibleTime = 0.5f;
    public void Init()
    {
        AccessoryPassive.Instance.Clear();
        nowExp = 0;
        nextDamageTime = 0f;
        isDead = false;
        level = 1;
        nextLevelExp = LevelManager.Instance.GetNextLevelExp(level);
        LevelUpManager.Instance.selectCount = 0;
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

    public bool DecreaseHp(int damage, BaseEnemyObject meleeAttacker = null)
    {
        if (isDead || damage <= 0 || Time.time < nextDamageTime)
            return false;

        nextDamageTime = Time.time + invincibleTime;
        int armor = Mathf.Max(0, GetArmor());
        float damageTakenMultiplier = 10f / (armor + 10f);
        int actualDamage = Mathf.Max(1, Mathf.CeilToInt(damage * damageTakenMultiplier));
        nowHp = Mathf.Max(0, nowHp - actualDamage);
        AudioManager.Instance.PlaySound(SoundCue.PlayerHurt, 0.9f);
        CameraShake.ShakeMainCamera();
        playerPos.GetComponent<PlayerController>().animator.SetTrigger("Hurt");
        PlayerController controller = playerPos == null ? null : playerPos.GetComponentInParent<PlayerController>();
        if (controller != null)
            controller.RunDamageEvent();
        if (meleeAttacker != null)
            AccessoryPassive.Instance.ReflectMeleeDamage(meleeAttacker);
        if (nowHp <= 0)
            Dead();
        return true;
    }

    private void Dead()
    {
        if (AccessoryPassive.Instance.isBuyChoker && !AccessoryPassive.Instance.isChokerTrigger)
        {
            nextDamageTime = Time.time + 1f;
            nowHp = 1;
            AccessoryPassive.Instance.isChokerTrigger = true;
            return;
        }
        isDead = true;
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound(SoundCue.GameOver, 0.8f);
        //播放死亡动画
        playerPos.GetComponent<PlayerController>().animator.SetTrigger("Death");
        GameObject.Destroy(playerPos.gameObject, 2f);
        UIManager.Instance.ShowPanel<GameOverPanel>();
    }
    public void AddExp(int exp)
    {
        nowExp += exp;
        LevelUp();
    }

    public void Heal(int amount)
    {
        if (!isDead && nowHp > 0 && amount > 0)
            nowHp = Mathf.Min(nowHp + amount, hp);
    }

    public void Clear()
    {
        Init();
        System.Array.Clear(weaponList, 0, weaponList.Length);
        accessoryList.Clear();
        SetRole(null);
    }
}
