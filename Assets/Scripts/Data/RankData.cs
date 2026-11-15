using System;

[Serializable]
public class RankData
{
    public int rank;
    public string playerName;
    public string roleName;
    public int score;
    public int wave;
    public float remain; // 存活时间，单位秒。
    public string resName; // Role图集中的头像切片名。
    public bool isBest;
}
