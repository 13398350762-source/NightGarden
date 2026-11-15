using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : BaseManager<LevelUpManager>
{
    public GameObject[] items = new GameObject[4];
    public int refreshCount;
    public int refreshPrice;
    private List<LevelUpData>[] statsPools;
    private GameObject itemPrefab;
    private bool canSelect;
    public int selectCount;

    public int GetRefreshPrice()
    {
        refreshPrice = RandomManager.Instance.GetRefreshPrice(GameManager.Instance.Wave, refreshCount);
        return refreshPrice;
    }

    public void Refresh()
    {
        LoadItemPools();
        var choices = new List<LevelUpData>();
        var usedNames = new HashSet<string>();
        for (int i = 0; i < items.Length; i++)
        {
            int level = RandomManager.Instance.RandomLevel(PlayerInfoManager.Instance.GetLucky(), GameManager.Instance.Wave);
            List<LevelUpData> pool;
            do
            {
                pool = statsPools[level].FindAll(data => !usedNames.Contains(data.statsName));
                if (pool.Count > 0) break;
                level--;
            } while (level >= 1);
            if (pool.Count == 0)
                throw new System.InvalidOperationException("升级配置不足，无法生成4种不同属性的选项");
            LevelUpData choice = pool[Random.Range(0, pool.Count)];
            choices.Add(choice);
            usedNames.Add(choice.statsName);
        }
        Clear();
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = GameObject.Instantiate(itemPrefab);
            items[i].GetComponent<LevelUpItem>().Init(choices[i]);
        }
        canSelect = true;
        GetRefreshPrice();
    }

    private void LoadItemPools()
    {
        if (statsPools != null && itemPrefab != null) return;
        GameObject prefab = ResManager.Instance.LoadAsset<GameObject>("item", "LevelUpItem");
        if (prefab == null || prefab.GetComponent<LevelUpItem>() == null)
            throw new System.InvalidOperationException("item包中需要有挂载LevelUpItem脚本的LevelUpItem预制体");
        var pools = new List<LevelUpData>[5];
        for (int i = 1; i <= 4; i++) pools[i] = new List<LevelUpData>();
        foreach (LevelUpData data in Resources.LoadAll<LevelUpData>("ScriptableObjects/LevelUpStats"))
            if (data.level >= 1 && data.level <= 4) pools[data.level].Add(data);
        if (pools[1].Count < 4)
            throw new System.InvalidOperationException("普通等级至少需要4种升级属性，请先生成LevelUpStats配置");
        statsPools = pools;
        itemPrefab = prefab;
    }

    public bool Select(LevelUpItem item)
    {
        if (!canSelect || item == null || System.Array.IndexOf(items, item.gameObject) < 0)
            return false;
        LevelUpData data = item.data;
        float value = LevelUpData.GetValue(data.statsName, data.num);
        PlayerInfoManager player = PlayerInfoManager.Instance;
        switch (data.statsName)
        {
            case "armor": player.AddArmor(data.num); break;
            case "hp": player.AddHp(data.num); break;
            case "lucky": player.AddLucky(data.num); break;
            case "STR": player.AddSTR(data.num); break;
            case "AST": player.AddAST(data.num); break;
            case "recovery": player.AddRecovery(data.num); break;
            case "money": player.AddMoney(data.num); break;
            case "damage": player.AddDamage(value); break;
            case "atkSpeed": player.AddAtkSpeed(value); break;
            case "range": player.AddRange(value); break;
            case "moveSpeed": player.AddMoveSpeed(value); break;
            case "drain": player.AddDrain(value); break;
        }
        canSelect = false;
        foreach (GameObject obj in items)
            if (obj != null) obj.GetComponent<LevelUpItem>().btnSure.interactable = false;
        return true;
    }

    public void Clear()
    {
        canSelect = false;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                items[i].SetActive(false);
                GameObject.Destroy(items[i]);
                items[i] = null;
            }
        }
    }
}
