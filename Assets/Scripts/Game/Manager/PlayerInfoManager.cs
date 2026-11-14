using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerInfoManager : BaseManager<PlayerInfoManager>
{
    public  List<AccessoryData> accessoryList = new List<AccessoryData>();
    public WeaponData[] weaponList = new WeaponData[6];

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
}
