using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class Main : MonoBehaviour
{
    public BaseWeaponObject weapon;
    public testEnemy testEnemy;
    void Start()
    {
        ResManager.Instance.LoadRes<SpriteAtlas>("atlas", "Components");
        UIManager.Instance.ShowPanel<LoginPanel>();
        Test();
    }

    void Update()
    {
        
    }

    private void Test()
    {
        
    }
}
