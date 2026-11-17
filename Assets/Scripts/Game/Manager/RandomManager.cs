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
        return Mathf.Clamp(1 + (Mathf.Max(1, wave) - 1) / 3, 1, 10);
    }

    public int GetEnemyHp(int baseHp, int wave, bool isBoss = false)
    {
        wave = Mathf.Max(1, wave);
        float scale = isBoss ? 1f + (Mathf.Max(1, wave / 10) - 1) * 0.6f : 1f + (wave - 1) * 0.12f;
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
        return Random.Range(min, max + 1);
    }

    public bool RandomEnemyPosition(Bounds bounds, float radius, out Vector2 position)
    {
        position = Vector2.zero;
        if (bounds.size.x <= radius * 2f || bounds.size.y <= radius * 2f) return false;
        Transform player = PlayerInfoManager.Instance.playerPos;
        for (int i = 0; i < 50; i++)
        {
            Vector2 point = new Vector2(Random.Range(bounds.min.x + radius, bounds.max.x - radius), Random.Range(bounds.min.y + radius, bounds.max.y - radius));
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

    //抽取商品等级
    public int RandomLevel(int lucky, int wave)
    {
        float t = Mathf.Clamp01(
            (Mathf.Max(wave, 1) - 1) / 25f
            + Mathf.Max(lucky, 0) / 200f);

        float p1 = Mathf.Lerp(0.70f, 0.15f, t);
        float p2 = Mathf.Lerp(0.23f, 0.35f, t);
        float p3 = Mathf.Lerp(0.06f, 0.30f, t);

        float value = Random.value;

        if (value < p1) return 1;
        if (value < p1 + p2) return 2;
        if (value < p1 + p2 + p3) return 3;
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
