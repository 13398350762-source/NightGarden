using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public SpriteRenderer renderRole;
    public Animator animator;
    public Transform posParent;
    public Transform target;
    private Transform[] posList;
    internal Vector2 moveDir;
    private int weaponRefreshVersion;
    private Rigidbody2D rb;
    private event UnityAction damageEvent;
    private float recoveryTime;


    public static bool IsPlayerBody(Collider2D other)
    {
        if (other == null || other.isTrigger) return false;
        if (other.GetComponentInParent<BaseWeaponObject>() != null || other.GetComponentInParent<BaseBullet>() != null) return false;
        PlayerController player = other.GetComponentInParent<PlayerController>();
        return player != null && other.attachedRigidbody != null && other.attachedRigidbody.gameObject == player.gameObject;
    }

    public void Init()
    {
        recoveryTime = 0f;
        moveDir = Vector2.zero;
        transform.position = Vector2.zero;
        posList = new Transform[6];
        for (int i = 0; i < 6; i++)
        {
            posList[i] = posParent.GetChild(i);
        }
        string name=PlayerInfoManager.Instance.GetRole().roleID;
        PlayerInfoManager.Instance.playerPos = target;
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
        PlayerInfoManager player = PlayerInfoManager.Instance;
        rb.velocity = moveDir * player.GetMoveSpeed();
        if (animator != null)
            animator.SetBool("IsMoving", rb.velocity.sqrMagnitude > 0.0001f);

        if (renderRole != null && Mathf.Abs(moveDir.x) > 0.01f)
            renderRole.flipX = moveDir.x < 0f;
        if (!GameManager.Instance.IsRunning || player.isDead || player.GetRecovery() <= 0)
            return;
        recoveryTime += Time.deltaTime;
        if (recoveryTime >= 1f)
        {
            int seconds = Mathf.FloorToInt(recoveryTime);
            recoveryTime -= seconds;
            player.Heal(player.GetRecovery() * seconds);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject coin = collision.gameObject;
        if (!coin.activeInHierarchy || !coin.CompareTag("Money")) return;

        string poolName = coin.name.Replace("(Clone)", "").Trim();
        int value;
        switch (poolName)
        {
            case "MoneyObject1": value = 1; break;
            case "MoneyObject2": value = 5; break;
            case "MoneyObject3": value = 10; break;
            default: return;
        }

        coin.SetActive(false);
        PlayerInfoManager.Instance.AddMoney(value);
        GameManager.Instance.Collection += value;
        AudioManager.Instance.PlaySound(SoundCue.Coin, 0.55f, 0.075f);
        PoolManager.Instance.PushObj(poolName, coin);
    }
    public void AddDamageEvent(UnityAction action)
    {
        damageEvent += action;
    }
    public void RemoveDamageEvent(UnityAction action)
    {
        damageEvent -= action;
    }
    public void RunDamageEvent()
    {
        damageEvent?.Invoke();
    }
}
