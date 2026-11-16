using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class CrystalFog : MonoBehaviour
{
    private float internalTime;
    private int damage;
    private int repel;
    private float range;
    private float lastTime=3f;
    private float duringTime=0f;
    private bool canAttack = true;
    private SpriteRenderer imgFog;
    private SpriteRenderer imgMist;
    private Vector3 baseScale;
    private Vector3 fogScale;
    private Quaternion baseRotation;
    private Color fogColor;
    private float animationTime;
    private bool isReady = false;
    private int loadVersion;
    private HashSet<BaseEnemyObject> hitEnemies = new HashSet<BaseEnemyObject>();

    private void Awake()
    {
        imgFog = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        baseRotation = transform.localRotation;
        fogColor = imgFog.color;
        imgFog.sortingLayerName = "Bullet";
        imgFog.enabled = false;

        GameObject mist = new GameObject("Mist");
        mist.transform.SetParent(transform, false);
        imgMist = mist.AddComponent<SpriteRenderer>();
        imgMist.sprite = imgFog.sprite;
        imgMist.sharedMaterial = imgFog.sharedMaterial;
        imgMist.sortingLayerName = "Bullet";
        imgMist.sortingOrder = imgFog.sortingOrder + 1;
        imgMist.flipX = true;
        imgMist.color = new Color(fogColor.r, fogColor.g, fogColor.b, 0f);
        imgMist.enabled = false;
    }

    public void Init(int damage,float range,int repel)
    {
        isReady = false;
        imgFog.enabled = false;
        imgMist.enabled = false;
        int version = ++loadVersion;
        internalTime = 0.5f;
        this.damage = damage;
        this.repel = repel;
        this.range = range;
        fogScale = baseScale * Mathf.Sqrt(Mathf.Sqrt(Mathf.Max(0f, range)));
        transform.localScale = fogScale;
        lastTime = 3f;
        duringTime = 0f;
        animationTime = 0f;
        canAttack = true;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Weapon", (atlas) => {
            if (this == null || version != loadVersion || !gameObject.activeInHierarchy)
                return;
            Sprite sprite = atlas != null ? atlas.GetSprite("lantern_fog") : null;
            if (sprite == null)
            {
                Debug.LogError("Weapon图集缺少lantern_fog，请重新打包atlas。");
                PoolManager.Instance.PushObj("CrystalFog", gameObject);
                return;
            }
            imgFog.sprite = sprite;
            imgMist.sprite = sprite;
            imgFog.enabled = true;
            imgMist.enabled = true;
            isReady = true;
            AnimateFog();
        });
    }

    private void AnimateFog()
    {
        float fade = Mathf.Clamp01(animationTime / 0.2f) * Mathf.Clamp01(lastTime / 0.45f);
        float wave = Mathf.Sin(animationTime * 2.4f);
        transform.localScale = fogScale * (1f + wave * 0.04f);
        transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, wave * 4f);
        imgFog.color = new Color(fogColor.r, fogColor.g, fogColor.b, fogColor.a * fade * 0.72f);
        float width = imgFog.sprite.bounds.size.x;
        imgMist.transform.localPosition = new Vector3(Mathf.Sin(animationTime * 1.7f) * width * 0.035f,
            Mathf.Cos(animationTime * 1.3f) * width * 0.025f, 0f);
        imgMist.transform.localRotation = Quaternion.Euler(0f, 0f, -wave * 7f);
        imgMist.transform.localScale = Vector3.one * (0.94f - wave * 0.025f);
        imgMist.color = new Color(fogColor.r, fogColor.g, fogColor.b,
            fogColor.a * fade * (0.2f + (wave + 1f) * 0.07f));
    }

    private void Attack()
    {
        hitEnemies.Clear();
        Collider2D[] collider2Ds=Physics2D.OverlapCircleAll(this.transform.position, range);
        foreach (var item in collider2Ds)
        {
            BaseEnemyObject enemy = item.GetComponentInParent<BaseEnemyObject>();
            if (enemy != null && hitEnemies.Add(enemy))
            {
                Vector2 dir = ((Vector2)(enemy.transform.position - transform.position)).normalized;
                enemy.GetRepel(repel, dir);
                enemy.GetDamage(damage);
            }
        }
    }

    private void Update()
    {
        if (!isReady)
            return;
        animationTime += Time.deltaTime;
        AnimateFog();
        if (canAttack)
        {
            //对范围内所有敌人造成伤害
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
        lastTime -= Time.deltaTime;
        if (lastTime <= 0)
        {
            isReady = false;
            PoolManager.Instance.PushObj("CrystalFog",this.gameObject);
            lastTime = 3f;
        }
    }

    private void OnDisable()
    {
        isReady = false;
        loadVersion++;
    }
}
