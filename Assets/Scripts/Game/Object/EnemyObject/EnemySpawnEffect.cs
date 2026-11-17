using UnityEngine;

public class EnemySpawnEffect : MonoBehaviour
{
    private LineRenderer[] lines;
    private float timer;

    public void Init()
    {
        if (lines == null) lines = GetComponentsInChildren<LineRenderer>();
        timer = 0f;
        foreach (var line in lines) line.enabled = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (lines == null) return;
        bool visible = Mathf.FloorToInt(timer / 0.12f) % 2 == 0;
        foreach (var line in lines) line.enabled = visible;
    }
}
