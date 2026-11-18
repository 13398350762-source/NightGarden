using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    private UserData userData;
    public List<RankData> dataList = new List<RankData>();
    public DataManager()
    {
        if (userData == null)
            userData = new UserData();
        userData.LoadData();
        LoadRankData();
    }

    public void AddUser(UserInfo info)
    {
        userData.AddData(info);
    }

    public void RemoveUser(string name)
    {
        userData.RemoveData(name);
    }
    public bool CheckUser(string name)
    {
        return userData.userDic.ContainsKey(name);
    }
    public bool CheckLogin(string name, string password)
    {
        return userData.userDic.ContainsKey(name) ? (userData.userDic[name].password==password?true:false):false;
    }

    public UserInfo GetUser(string name)
    {
        return userData.userDic[name];
    }
    public void SaveUserData()
    {
        userData.SaveUserData();
    }
    private void LoadRankData()
    {
       dataList = JsonManager.Instance.Load<List<RankData>>("RankData");
       if(dataList==null)
           dataList = new List<RankData>();
       UpdateRanks();
    }
    public void AddRankData(RankData data)
    {
        if (data == null)
            return;
        dataList.Add(data);
        UpdateRanks();
        JsonManager.Instance.Save(dataList, "RankData");
    }

    private void UpdateRanks()
    {
        dataList.RemoveAll(data => data == null);
        List<RankData> sorted = dataList.OrderByDescending(data => data.score).ToList();
        dataList.Clear();
        dataList.AddRange(sorted);

        var bestPlayers = new HashSet<string>();
        for (int i = 0; i < dataList.Count; i++)
        {
            RankData data = dataList[i];
            data.rank = i + 1;
            data.isBest = bestPlayers.Add(data.playerName);
        }
    }
}
