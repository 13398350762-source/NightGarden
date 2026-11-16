using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalObject : BaseWeaponObject
{
    public override void Attack()
    {
        Vector3 position = transform.position;
        int damage = atk;
        int knockback = repel;
        float attackRange = range;
        PoolManager.Instance.GetObj("CrystalFog", "bullet",  (obj) =>{
            if (obj == null)
                return;
            Transform trans = (obj as GameObject).transform;
            trans.position = position;
            trans.GetComponent<CrystalFog>().Init(damage,attackRange,knockback);
        });
    }

    private void Update()
    {

        if (canAttack)
        {
            Attack();
            canAttack = false;
        }
        else
        {
            duringTime += Time.deltaTime;

            if (duringTime >= internalTime)
            {
                duringTime = 0;
                canAttack = true;
            }
        }
    }
}
