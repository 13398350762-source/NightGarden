using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UI;

public abstract class BaseWeaponObject : MonoBehaviour
{
    public SpriteRenderer imgWeapon;
    protected GameObject nearestEnemy = null;
    public Transform pos;
    protected readonly Vector3 restLocalPosition = new Vector3(0f, 0f, 10f);
    protected string weaponID;
    protected string resEff;
    protected string resName;
    protected string bullet;
    protected float internalTime;
    protected float range;
    protected int atk;
    protected int repel;
    protected float duringTime=0;
    protected bool canAttack=true;
    virtual public void Init(WeaponData data)
    {
        if (pos != null)
            transform.SetParent(pos, false);
        transform.localPosition = restLocalPosition;
        imgWeapon.sortingLayerName = "Weapon";

        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "weapon", (atlas) =>{
            imgWeapon.sprite=atlas.GetSprite(data.resName);
        });
        weaponID = data.weaponID;
        resEff = data.resEff;
        resName = data.resName;
        bullet = data.bullet;
        internalTime = data.internalTime / PlayerInfoManager.Instance.GetAtkSpeed();
        range = data.range * PlayerInfoManager.Instance.GetRange();
        repel = data.repel;
        atk=data.atk;
        switch (data.damageType)
        {
            case DamageType.AST:
                atk += PlayerInfoManager.Instance.GetAST();
                break;
            case DamageType.STR:
                atk += PlayerInfoManager.Instance.GetSTR();
                break;
        }
        atk = Mathf.RoundToInt(atk * PlayerInfoManager.Instance.GetDamage());
    }
    public abstract void Attack();
    protected GameObject GetNearestEnemy()
    {
        Collider2D[] collider2ds = Physics2D.OverlapCircleAll(transform.position, range);

        float minDistance = float.MaxValue;
        GameObject result = null;

        foreach (var item in collider2ds)
        {
            if (!item.CompareTag("Enemy"))
                continue;

            float distance = Vector2.Distance(item.transform.position, transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                result = item.gameObject;
            }
        }

        return result;
    }
}
