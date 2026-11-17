using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : BaseManager<LevelManager>
{
    public int GetNextLevelExp(int currentLevel)
    {
        currentLevel = Mathf.Max(1, currentLevel);
        return 10 + (currentLevel - 1) * 5;
    }

    public int GetEnemyExp(int baseExp, int wave)
    {
        baseExp = Mathf.Max(0, baseExp);
        wave = Mathf.Max(1, wave);
        return Mathf.RoundToInt(baseExp * (1f + (wave - 1) * 0.1f));
    }
}
