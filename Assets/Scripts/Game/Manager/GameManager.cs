using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : BaseManager<GameManager>
{
    public int Wave;
    public int Score;
    public float Remain;
    public int Death;
    public int Collection;
    public float nowWaveRemainTime;
    public void Clear()
    {
        Wave = 0; 
        Score = 0;
        Remain = 0;
        Death = 0;
        Collection = 0;
    }
}
