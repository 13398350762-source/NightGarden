using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : BaseManager<UserManager>
{
    private UserInfo nowUser=new UserInfo();

    public void SetNowUser(UserInfo info)
    {
        nowUser = info;
    }
    public void SetChiliUnLock()
    {
        nowUser.SetChiliUnLock();
        DataManager.Instance.SaveUserData();
    }
    public void SetPeaUnLock()
    {
        nowUser.SetPeaUnLock();
        DataManager.Instance.SaveUserData();
    }

    public bool IsChiliUnlock()
    {
        return nowUser.isUnLockChili;
    }

    public bool IsPeaUnlock()
    {
        return nowUser.isUnLockPea;
    }
    public UserInfo GetNowUser()
    {
        return nowUser;
    }

}
