using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
    private UserData userData;
    public DataManager()
    {
        if (userData == null)
            userData = new UserData();
        userData.LoadData();
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
}
