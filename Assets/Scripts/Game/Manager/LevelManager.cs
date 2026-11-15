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
}
