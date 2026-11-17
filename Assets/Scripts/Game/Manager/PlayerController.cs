using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer renderRole;
    public Transform posParent;
    public Transform target;
    private Transform[] posList;
    internal Vector2 moveDir;
    private int weaponRefreshVersion;
    private Rigidbody2D rb;

    public static bool IsPlayerBody(Collider2D other)
    {
        if (other == null || other.isTrigger) return false;
        if (other.GetComponentInParent<BaseWeaponObject>() != null || other.GetComponentInParent<BaseBullet>() != null) return false;
        PlayerController player = other.GetComponentInParent<PlayerController>();
        return player != null && other.attachedRigidbody != null && other.attachedRigidbody.gameObject == player.gameObject;
    }

    public void Init()
    {
        moveDir = Vector2.zero;
        transform.position = Vector2.zero;
        posList = new Transform[6];
        for (int i = 0; i < 6; i++)
        {
            posList[i] = posParent.GetChild(i);
        }
        string name=PlayerInfoManager.Instance.GetRole().roleID;
        PlayerInfoManager.Instance.playerPos = target;
        //加载人物图片
        RefreshWeapon();
        PlayerInfoManager.Instance.Init();
        PlayerInfoManager.Instance.nowHp = PlayerInfoManager.Instance.GetHp();
        GameManager.Instance.Init();
    }
    public void Move(InputAction.CallbackContext context)
    {
        EventCallBack.Instance.Trigger(nameof(EventCallBack.MovePlayer), new EventValue<Vector2>(this, context.ReadValue<Vector2>()));
    }
    public void Pause(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        EventCallBack.Instance.Trigger(nameof(EventCallBack.PauseGame));
    }

    public void RefreshWeapon()
    {
        int version = ++weaponRefreshVersion;
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
                    if (this == null || targetPos == null || version != weaponRefreshVersion)
                    {
                        if (weapon != null) Destroy(weapon);
                        return;
                    }
                    (weapon as GameObject).transform.SetParent(targetPos, false);
                    weapon.GetComponent<BaseWeaponObject>().Init(data);
                });
            }
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnDisable()
    {
        weaponRefreshVersion++;
        moveDir = Vector2.zero;
    }
    private void Update()
    {
        rb.velocity = moveDir * PlayerInfoManager.Instance.GetMoveSpeed();
    }
}
