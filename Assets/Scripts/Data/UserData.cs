using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    public Dictionary<string, UserInfo> userDic=new Dictionary<string, UserInfo>();

    public void AddData(UserInfo info)
    {
        userDic.Add(info.name, info);
        SaveUserData();
    }
    public void RemoveData(string name)
    {
        userDic.Remove(name);
        SaveUserData();
    }
    public void LoadData()
    {
        userDic = JsonManager.Instance.Load<Dictionary<string, UserInfo>>("UserData");
        if(userDic==null)
            userDic = new Dictionary<string, UserInfo>();
    }
    private void SaveUserData()
    {
        JsonManager.Instance.Save(userDic, "UserData");
    }
}

public class UserInfo
{
    public string name;
    public string password;
    public UserInfo(string name, string password)
    {
        this.name = name;
        this.password = password;
    }

    public UserInfo() { }
}
