using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Main : MonoBehaviour
{
    private BaseEnemyObject enemy;
    private PlayerController player;
    void Start()
    {
        EventCallBack.Instance.Init();
        ResManager.Instance.LoadRes<SpriteAtlas>("atlas", "Components");
        UIManager.Instance.ShowPanel<LoginPanel>();
        //Test();
    }

    void Update()
    {
        
    }

    private void Test()
    {
        enemy = FindObjectOfType<BaseEnemyObject>();
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            // 正常流程会异步加载玩家，由 PlayerController.Init 开始波次。
            return;
        }
        PlayerInfoManager.Instance.Init();
        if (PlayerInfoManager.Instance.GetRole() == null)
            PlayerInfoManager.Instance.SetRole(Resources.Load<RoleData>("ScriptableObjects/RoleDatas/萝卜骑士"));
        PlayerInfoManager.Instance.nowHp = Mathf.Max(1, PlayerInfoManager.Instance.GetHp());
        PlayerInfoManager.Instance.playerPos = player.target != null ? player.target : player.transform;
        GameManager.Instance.Init();
    }
}
