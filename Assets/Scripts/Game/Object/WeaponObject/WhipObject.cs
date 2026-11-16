using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WhipObject : BaseWeaponObject
{
    // 挥出时间
    private float attackDuration;
    // 收回时间
    private float returnDuration;
    private bool isAttacking = false;
    private Vector2 attackDirection;
    private HashSet<BaseEnemyObject> hitEnemies = new HashSet<BaseEnemyObject>();
    private const int pointCount = 33;
    private Vector3[] whipPoints = new Vector3[pointCount];
    private SpriteRenderer imgHandle;
    private Mesh whipMesh;
    private MeshRenderer whipRenderer;
    private Material whipMaterial;
    private Vector3[] whipVertices = new Vector3[pointCount * 2];
    private Vector2[] whipUV = new Vector2[pointCount * 2];
    private int[] whipTriangles = new int[(pointCount - 1) * 6];
    private Color[] whipColors = new Color[pointCount * 2];

    public override void Init(WeaponData data)
    {
        base.Init(data);
        // 战斗中使用独立握柄，鞭身在卷起和展开之间连续变化。
        imgWeapon.enabled = false;
        ResManager.Instance.LoadResAsync<SpriteAtlas>("atlas", "Weapon", (atlas) => {
            if (this == null)
                return;
            Sprite handle = atlas.GetSprite("whip_handle");
            Sprite vine = atlas.GetSprite("whip_vine");
            if (handle == null || vine == null)
            {
                Debug.LogError("Weapon图集缺少whip_handle或whip_vine，请重新打包atlas。");
                return;
            }
            CreateWhipVisual(handle, vine);
        });
    }

    public override void Attack()
    {
        if (isAttacking || !canAttack || nearestEnemy == null || range <= 0f || whipRenderer == null)
            return;
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        attackDuration = Mathf.Max(0.08f, internalTime * 0.3f);
        returnDuration = Mathf.Max(0.06f, internalTime * 0.18f);
        float windupDuration = Mathf.Max(0.04f, internalTime * 0.12f);
        isAttacking = true;
        canAttack = false;
        duringTime = 0f;
        hitEnemies.Clear();
        transform.localPosition = restLocalPosition;
        attackDirection = ((Vector2)(nearestEnemy.transform.position - transform.position)).normalized;
        if (attackDirection.sqrMagnitude < 0.001f)
            attackDirection = Vector2.right;
        transform.rotation = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg);
        SetWhipVisible(true);
        float timer = 0f;
        while (timer < windupDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / windupDuration);
            DrawWhip(-90f, Mathf.SmoothStep(0f, 1f, t), Mathf.Lerp(120f, 0f, t));
            yield return null;
        }
        // 从右侧扫到左侧，覆盖面向目标的前方180度。
        timer = 0f;
        while (timer < attackDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / attackDuration);
            float angle = Mathf.Lerp(-90f, 90f, t);
            DrawWhip(angle, 1f, Mathf.Sin(t * Mathf.PI) * 35f);
            CheckHit(angle);
            yield return null;
        }
        timer = 0f;
        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / returnDuration);
            DrawWhip(Mathf.Lerp(90f, 0f, t), 1f - Mathf.SmoothStep(0f, 1f, t), t * 150f);
            yield return null;
        }
        transform.localPosition = restLocalPosition;
        isAttacking = false;
        DrawWhip(0f, 0f, 0f);
    }

    private void CheckHit(float sweptAngle)
    {
        Vector2 center = transform.position;
        foreach (Collider2D other in Physics2D.OverlapCircleAll(center, range))
        {
            BaseEnemyObject enemy = other.GetComponentInParent<BaseEnemyObject>();
            if (enemy == null || !enemy.gameObject.activeInHierarchy || hitEnemies.Contains(enemy))
                continue;
            Vector2 offset = other.ClosestPoint(center) - center;
            if (offset.sqrMagnitude > range * range)
                continue;
            float angle = Vector2.SignedAngle(attackDirection, offset);
            if (offset.sqrMagnitude > 0.0001f && (angle < -90f || angle > sweptAngle + 0.01f))
                continue;
            hitEnemies.Add(enemy);
            Vector2 direction = (Vector2)enemy.transform.position - center;
            if (direction.sqrMagnitude < 0.001f)
                direction = attackDirection;
            enemy.GetDamage(atk);
            if (enemy != null && enemy.gameObject.activeInHierarchy)
                enemy.GetRepel(repel, direction.normalized);
        }
    }

    private void CreateWhipVisual(Sprite handle, Sprite vine)
    {
        if (whipRenderer != null)
            return;

        GameObject handleObj = new GameObject("WhipHandle");
        handleObj.transform.SetParent(transform, false);
        imgHandle = handleObj.AddComponent<SpriteRenderer>();
        imgHandle.sprite = handle;
        imgHandle.sharedMaterial = imgWeapon.sharedMaterial;
        imgHandle.sortingLayerID = imgWeapon.sortingLayerID;
        imgHandle.sortingOrder = imgWeapon.sortingOrder + 1;

        GameObject vineObj = new GameObject("WhipVine");
        vineObj.transform.SetParent(transform, false);
        whipRenderer = vineObj.AddComponent<MeshRenderer>();
        whipMaterial = new Material(imgWeapon.sharedMaterial);
        whipMaterial.mainTexture = vine.texture;
        whipRenderer.sharedMaterial = whipMaterial;
        whipRenderer.sortingLayerID = imgWeapon.sortingLayerID;
        whipRenderer.sortingOrder = imgWeapon.sortingOrder;
        whipMesh = new Mesh { name = "WhipVine" };
        whipMesh.MarkDynamic();
        vineObj.AddComponent<MeshFilter>().sharedMesh = whipMesh;
        Rect rect = vine.textureRect;
        for (int i = 0; i < pointCount; i++)
        {
            // 跳过长条两端的透明留白，避免连接处出现空隙。
            float u = Mathf.Lerp(rect.xMin + rect.width * 0.008f,
                rect.xMax - rect.width * 0.008f, i / (float)(pointCount - 1)) / vine.texture.width;
            whipUV[i * 2] = new Vector2(u, rect.yMin / vine.texture.height);
            whipUV[i * 2 + 1] = new Vector2(u, rect.yMax / vine.texture.height);
            whipColors[i * 2] = Color.white;
            whipColors[i * 2 + 1] = Color.white;
            if (i == pointCount - 1)
                continue;
            int index = i * 6;
            int vertex = i * 2;
            whipTriangles[index] = vertex;
            whipTriangles[index + 1] = vertex + 2;
            whipTriangles[index + 2] = vertex + 1;
            whipTriangles[index + 3] = vertex + 1;
            whipTriangles[index + 4] = vertex + 2;
            whipTriangles[index + 5] = vertex + 3;
        }
        whipRenderer.enabled = true;
        DrawWhip(0f, 0f, 0f);
    }

    private void DrawWhip(float sweepAngle, float extension, float bend)
    {
        if (whipRenderer == null)
            return;

        Vector3 origin = transform.position;
        float aimAngle = isAttacking
            ? Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg
            : transform.eulerAngles.z;
        float handleLength = Mathf.Min(range * 0.15f, 1.2f);
        float attackWidth = Mathf.Min(range * 0.075f, 0.8f);
        float width = Mathf.Lerp(Mathf.Min(attackWidth, handleLength * 0.45f), attackWidth, extension);
        float handleAngle = (aimAngle + sweepAngle) * Mathf.Deg2Rad;
        Vector3 handleDirection = new Vector3(Mathf.Cos(handleAngle), Mathf.Sin(handleAngle), 0f);
        Vector3 handleNormal = new Vector3(-handleDirection.y, handleDirection.x, 0f);
        // 鞭根稍微压进握柄连接头内，握柄位于上层，遮住接缝。
        float anchorLength = handleLength * 0.94f;
        Vector3 anchor = origin + handleDirection * anchorLength;
        imgHandle.transform.position = origin;
        imgHandle.transform.rotation = Quaternion.Euler(0f, 0f, aimAngle + sweepAngle);
        // 换算成世界长度，不受pos或武器对象缩放影响。
        Vector3 scale = imgHandle.transform.lossyScale;
        imgHandle.transform.localScale = Vector3.Scale(imgHandle.transform.localScale,
            new Vector3(handleLength / Mathf.Max(0.001f, imgHandle.sprite.bounds.size.x * Mathf.Abs(scale.x)),
                handleLength / Mathf.Max(0.001f, imgHandle.sprite.bounds.size.x * Mathf.Abs(scale.y)), 1f));

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            float angle = (aimAngle + sweepAngle - Mathf.Sin(t * Mathf.PI) * bend) * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
            float radius = Mathf.Lerp(anchorLength, Mathf.Max(anchorLength, range - width * 0.5f), t);
            Vector3 extendedPoint = origin + direction * radius;

            // 从握柄连接头起卷成螺旋；首点始终固定在anchor。
            float coilRadius = handleLength * 0.65f;
            float coilAngle = -Mathf.PI * 0.5f + t * Mathf.PI * 3f;
            float spiralRadius = coilRadius * (1f - t * 0.78f);
            Vector3 coiledPoint = anchor
                + handleDirection * (Mathf.Cos(coilAngle) * spiralRadius)
                + handleNormal * (coilRadius + Mathf.Sin(coilAngle) * spiralRadius);
            whipPoints[i] = Vector3.Lerp(coiledPoint, extendedPoint, extension);
        }
        for (int i = 0; i < pointCount; i++)
        {
            Vector3 tangent = (whipPoints[Mathf.Min(i + 1, pointCount - 1)]
                - whipPoints[Mathf.Max(i - 1, 0)]).normalized;
            Vector3 normal = new Vector3(-tangent.y, tangent.x, 0f) * width * 0.5f;
            // 显示和命中均使用range世界半径，边缘也不超出攻击范围。
            Vector3 lower = Vector3.ClampMagnitude(whipPoints[i] - normal - origin, range);
            Vector3 upper = Vector3.ClampMagnitude(whipPoints[i] + normal - origin, range);
            whipVertices[i * 2] = transform.InverseTransformPoint(origin + lower);
            whipVertices[i * 2 + 1] = transform.InverseTransformPoint(origin + upper);
        }
        whipMesh.vertices = whipVertices;
        whipMesh.uv = whipUV;
        whipMesh.triangles = whipTriangles;
        whipMesh.colors = whipColors;
        whipMesh.RecalculateBounds();
    }

    private void SetWhipVisible(bool visible)
    {
        if (whipRenderer != null)
            whipRenderer.enabled = visible;
    }

    private void OnEnable()
    {
        if (whipRenderer != null)
        {
            SetWhipVisible(true);
            DrawWhip(0f, 0f, 0f);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        SetWhipVisible(false);
        hitEnemies.Clear();
        isAttacking = false;
        canAttack = true;
        duringTime = 0f;
        transform.localPosition = restLocalPosition;
    }

    private void OnDestroy()
    {
        if (whipMesh != null)
            Destroy(whipMesh);
        if (whipMaterial != null)
            Destroy(whipMaterial);
    }

    private void Update()
    {
        if (!isAttacking)
        {
            nearestEnemy = GetNearestEnemy();
            if (nearestEnemy != null)
            {
                Vector3 direction = nearestEnemy.transform.position - transform.position;
                transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            }
            else
            {
                transform.localEulerAngles = Vector3.zero;
            }
        }
        if (!isAttacking && imgHandle != null)
            DrawWhip(0f, 0f, 0f);

        if (canAttack)
        {
            if (!isAttacking && nearestEnemy != null)
                Attack();
        }
        else
        {
            duringTime += Time.deltaTime;
            if (duringTime >= internalTime)
            {
                duringTime = 0f;
                canAttack = true;
            }
        }
    }
}
