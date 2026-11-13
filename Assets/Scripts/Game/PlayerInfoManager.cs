using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoManager : BaseManager<PlayerInfoManager>
{
    private RoleData selectRole=null;
    private WeaponData selectWeapon=null;
    public void SetRole(RoleData data)
    {
        selectRole = data;
    }
    public RoleData GetRole()
    {
        return selectRole;
    }
    public void SetWeapon(WeaponData data)
    {
        selectWeapon = data;
    }
    public WeaponData GetWeapon()
    {
        return selectWeapon;
    }
}
