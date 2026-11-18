using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager : BaseManager<RandomManager>
{
    private readonly string[] enemies = { "GrubEnemy", "SnailEnemy", "MothEnemy", "BeetleEnemy", "SlimeEnemy", "RootSpiderEnemy", "PuffballEnemy", "RottenPumpkinEnemy", "PotMimicEnemy", "BeetleCaptainEnemy", "SporeShamanEnemy", "CrystalSlimeEnemy" };
    private readonly string[] bosses = { "RotrootKingEnemy", "MothQueenEnemy", "CompostCauldronEnemy", "EclipsePumpkinEnemy" };

    public int GetEnemyTypeCount(int wave)
    {
        return Mathf.Clamp(1 + (Mathf.Max(1, wave) - 1) / 2, 1, enemies.Length);
    }

    public string RandomEnemy(int wave)
    {
        return enemies[Random.Range(0, GetEnemyTypeCount(wave))];
    }

    public string RandomBoss()
    {
        return bosses[Random.Range(0, bosses.Length)];
    }

    public int GetEnemyCount(int wave)
    {
        return Mathf.Clamp(3 + (Mathf.Max(1, wave) - 1) / 2, 3, 14);
    }

    public int GetEnemyHp(int baseHp, int wave, bool isBoss = false)
    {
        wave = Mathf.Max(1, wave);
        float scale = isBoss ? 1f + (Mathf.Max(1, wave / 10) - 1) * 0.6f : 1f + (wave - 1) * 0.10f;
        return Mathf.Max(1, Mathf.RoundToInt(baseHp * scale));
    }

    public int GetEnemyMoney(int baseExp, int wave, int lucky, bool isBoss = false)
    {
        if (baseExp <= 0) return 0;
        float waveScale = 1f + (Mathf.Max(1, wave) - 1) * 0.04f;
        float luckyScale = 1f + Mathf.Clamp(lucky, -100, 300) * 0.0025f;
        float value = baseExp * (isBoss ? 0.5f : 1f) * waveScale * luckyScale;
        int min = Mathf.Max(1, Mathf.FloorToInt(value * 0.7f));
        int max = Mathf.Max(min + 1, Mathf.CeilToInt(value * 1.2f));
        float reducedDrop = Random.Range(min, max + 1) * 0.65f;
        int coins = Mathf.FloorToInt(reducedDrop);
        // 小额掉落按小数部分补发，长期平均仍是原掉落的 65%。
        return coins + (Random.value < reducedDrop - coins ? 1 : 0);
    }

    public bool RandomEnemyPosition(Bounds bounds, float radius, out Vector2 position, Camera camera = null)
    {
        position = Vector2.zero;
        Vector2 min = (Vector2)bounds.min + Vector2.one * radius;
        Vector2 max = (Vector2)bounds.max - Vector2.one * radius;
        if (camera != null)
        {
            const float margin = 0.1f;
            float depth = camera.WorldToViewportPoint(bounds.center).z;
            if (depth < camera.nearClipPlane || depth > camera.farClipPlane) return false;
            min = Vector2.Max(min, camera.ViewportToWorldPoint(new Vector3(margin, margin, depth)));
            max = Vector2.Min(max, camera.ViewportToWorldPoint(new Vector3(1f - margin, 1f - margin, depth)));
        }
        if (min.x >= max.x || min.y >= max.y) return false;

        Transform player = PlayerInfoManager.Instance.playerPos;
        for (int i = 0; i < 50; i++)
        {
            Vector2 point = new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
            if (player != null && Vector2.Distance(point, player.position) < radius + 5f) continue;
            if (Physics2D.OverlapCircle(point, radius, LayerMask.GetMask("Wall")) != null) continue;
            position = point;
            return true;
        }
        return false;
    }

    public int GetRefreshPrice(int wave, int refreshCount)
    {
        wave = Mathf.Max(wave, 1);
        refreshCount = Mathf.Max(refreshCount, 0);

        int basePrice = 2 + (wave - 1) / 2; // 每两波基础价格增加1。
        int priceIncrease = 1 + (wave - 1) / 5; // 每五波提高连续刷新的涨价幅度。
        return basePrice + refreshCount * priceIncrease;
    }

    // 商店和升级奖励共用等级概率，一级取剩余概率。
    public int RandomLevel(int lucky, int wave)
    {
        float t = Mathf.Clamp01(
            (Mathf.Max(wave, 1) - 1) / 25f
            + Mathf.Max(lucky, 0) / 200f);

        float rare = Mathf.Lerp(0.04f, 0.06f, t);
        float epic = Mathf.Lerp(0.006f, 0.02f, t);
        float legendary = Mathf.Lerp(0.001f, 0.004f, t);
        float common = 1f - rare - epic - legendary;

        float value = Random.value;

        if (value < common) return 1;
        if (value < common + rare) return 2;
        if (value < common + rare + epic) return 3;
        return 4;
    }

    public int RandomSellPrice(int level, int wave)
    {
        if (level < 1 || level > 4)
            throw new System.ArgumentOutOfRangeException(nameof(level));
        int[] minPrices = { 3, 5, 9, 15 };
        int[] maxPrices = { 4, 7, 12, 20 };

        float scale = 1f + (Mathf.Max(wave, 1) - 1) * 0.08f;
        int min = Mathf.RoundToInt(minPrices[level - 1] * scale);
        int max = Mathf.RoundToInt(maxPrices[level - 1] * scale);
        return Random.Range(min, max + 1);
    }

    //抽取商品价格
    public int RandomPrice(int level, int wave)
    {
        if (level < 1 || level > 4)
            throw new System.ArgumentOutOfRangeException(nameof(level));
        int[] minPrices = { 8, 14, 24, 40 };
        int[] maxPrices = { 12, 20, 34, 55 };

        float scale = 1f + (Mathf.Max(wave, 1) - 1) * 0.08f;

        int min = Mathf.RoundToInt(minPrices[level - 1] * scale);
        int max = Mathf.RoundToInt(maxPrices[level - 1] * scale);

        return Random.Range(min, max + 1);
    }
}
