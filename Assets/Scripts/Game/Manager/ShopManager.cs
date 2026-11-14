using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : BaseManager<ShopManager>
{
    //商品
    public  GameObject[] items = new GameObject[4];
    private List<WeaponData>[] weaponPools;
    private List<AccessoryData>[] accessoryPools;
    private GameObject weaponPrefab;
    private GameObject accessoryPrefab;
    public int refreshCount=0;
    public int refreshPrice;
    public int GetRefreshPrice()
    {
        refreshPrice=RandomManager.Instance.GetRefreshPrice(GameManager.Instance.Wave,refreshCount);
        return refreshPrice;
    }
    public void Refresh()
    {
        GetRefreshPrice();
        GameObject[] objs=new GameObject[4];
        int cnt=0;
        for (int i = 0; i < 4; i++)
        {
            if (items[i] != null && items[i].GetComponentInChildren<Toggle>(true).isOn)
                objs[i] = items[i];
            else
                cnt++;
        }

        List<GameObject> list=new List<GameObject>();
        //随机获得cnt个item
        GetItem(list, cnt,PlayerInfoManager.Instance.GetLucky(),GameManager.Instance.Wave);
        int pos = list.Count - 1;
        for (int i = 0; i < 4; i++)
        {
            if (objs[i] == null)
            {
                if (items[i] != null)
                {
                    items[i].SetActive(false);
                    GameObject.Destroy(items[i]);
                }
                objs[i] = list[pos];
                list.RemoveAt(pos);
                pos--;
            }
        }
        items = objs;
    }

    public void GetItem(List<GameObject> list, int cnt, int lucky,int wave)
    {
        if (cnt <= 0)
            return;
        LoadItemPools();

        for (int i = 0; i < cnt; i++)
        {
            int level = RandomManager.Instance.RandomLevel(lucky, wave);
            bool isWeapon = Random.value < 0.5f;

            // 同档某一类型为空时改抽另一类型；两类都为空才降低档位。
            while (level > 1 && weaponPools[level].Count == 0 && accessoryPools[level].Count == 0)
                level--;
            if (weaponPools[level].Count == 0)
                isWeapon = false;
            else if (accessoryPools[level].Count == 0)
                isWeapon = true;

            int price = RandomManager.Instance.RandomPrice(level, wave);
            GameObject item;
            if (isWeapon)
            {
                List<WeaponData> pool = weaponPools[level];
                WeaponData data = pool[Random.Range(0, pool.Count)];
                item = GameObject.Instantiate(weaponPrefab);
                item.GetComponent<WeaponShopItem>().Init(data, price);
            }
            else
            {
                List<AccessoryData> pool = accessoryPools[level];
                AccessoryData data = pool[Random.Range(0, pool.Count)];
                item = GameObject.Instantiate(accessoryPrefab);
                item.GetComponent<AccessoryShopItem>().Init(data, price);
            }
            item.GetComponentInChildren<Toggle>(true).SetIsOnWithoutNotify(false);
            list.Add(item);
        }
    }

    private void LoadItemPools()
    {
        if (weaponPools != null && accessoryPools != null && weaponPrefab != null && accessoryPrefab != null)
            return;

        // GetItem 需要立即填好列表，因此在抽取之前同步准备完整的资源池。
        GameObject weapon = ResManager.Instance.LoadAsset<GameObject>("item", "ShopItemWeapon");
        GameObject accessory = ResManager.Instance.LoadAsset<GameObject>("item", "ShopItemAccessory");
        if (weapon == null || accessory == null)
            throw new System.InvalidOperationException("item 包中缺少 ShopItemWeapon 或 ShopItemAccessory 预制体");

        List<WeaponData>[] weapons = new List<WeaponData>[5];
        List<AccessoryData>[] accessories = new List<AccessoryData>[5];
        for (int i = 1; i <= 4; i++)
        {
            weapons[i] = new List<WeaponData>();
            accessories[i] = new List<AccessoryData>();
        }
        foreach (WeaponData data in Resources.LoadAll<WeaponData>("ScriptableObjects/WeaponDatas"))
        {
            if (data.level >= 1 && data.level <= 4)
                weapons[data.level].Add(data);
        }
        string[] levels = { "普通", "稀有", "史诗", "传说" };
        foreach (AccessoryData data in Resources.LoadAll<AccessoryData>("ScriptableObjects/AccessoryData"))
        {
            int level = System.Array.IndexOf(levels, data.level) + 1;
            if (level > 0)
                accessories[level].Add(data);
        }
        if (weapons[1].Count == 0 && accessories[1].Count == 0)
            throw new System.InvalidOperationException("缺少普通档商品配置，无法向下回退");

        weaponPrefab = weapon;
        accessoryPrefab = accessory;
        weaponPools = weapons;
        accessoryPools = accessories;
    }
}
