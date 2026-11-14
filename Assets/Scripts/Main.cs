using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Main : MonoBehaviour
{
    void Start()
    {
        ResManager.Instance.LoadRes<SpriteAtlas>("atlas", "Components");
        PlayerInfoManager.Instance.SetRole(Resources.Load<RoleData>("ScriptableObjects/RoleDatas/萝卜骑士"));
        UIManager.Instance.ShowPanel<ShopPanel>();
    }

    void Update()
    {
        
    }
}
