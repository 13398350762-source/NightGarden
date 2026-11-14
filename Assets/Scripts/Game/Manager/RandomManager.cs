using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomManager : BaseManager<RandomManager>
{
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
