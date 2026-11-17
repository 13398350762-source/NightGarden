using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer renderRole;
    public Transform posParent;
    private Transform[] posList;
    private Vector2 moveDir;
    public void Init()
    {
        transform.position = Vector2.zero;
        posList = new Transform[6];
        for (int i = 0; i < 6; i++)
        {
            posList[i] = posParent.GetChild(i);
        }
        string name=PlayerInfoManager.Instance.GetRole().roleID;
        //加载人物图片
        RefreshWeapon();
    }
    public void Move(InputAction.CallbackContext context)
    {
        moveDir = context.ReadValue<Vector2>().normalized;
    }

    private void Update()
    {
        transform.Translate((Vector3)moveDir* PlayerInfoManager.Instance.GetMoveSpeed() * Time.deltaTime,Space.World);
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        UIManager.Instance.ShowPanel<PausePanel>();
    }

    public void RefreshWeapon()
    {
        for (int i = 0; i < 6; i++)
        {
            int num = posList[i].childCount;
            for (int j = num - 1; j >= 0; j--)
            {
                GameObject obj = posList[i].GetChild(j).gameObject;
                PoolManager.Instance.PushObj(obj.name, obj);
            }
            WeaponData data = PlayerInfoManager.Instance.weaponList[i];
            if (data != null)
            {
                Transform targetPos = posList[i];

                PoolManager.Instance.GetObj(data.weaponID, "weapon", (weapon) =>
                {
                    (weapon as GameObject).transform.SetParent(targetPos, false);
                    weapon.GetComponent<BaseWeaponObject>().Init(data);
                });
            }
        }
    }
}
