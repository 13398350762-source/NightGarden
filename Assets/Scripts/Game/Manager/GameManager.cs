using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : BaseManager<GameManager>
{
    public int Wave;
    public int Score;
    public float Remain;
    public int Death;
    public int Collection;
    public float nowWaveRemainTime;
    public float internalTime = 1f;
    private bool isGenerateBoss=false;
    public int maxEnemyCount = 50;
    public float waveTime = 60f;
    public float generateInterval = 5f;
    private bool isRunning;
    private int spawnVersion;
    private int pendingCount;
    private Bounds mapBounds;
    private List<Vector2> spawnPositions = new List<Vector2>();
    private List<GameObject> spawnEffects = new List<GameObject>();
    public bool IsRunning => isRunning;
    public void Clear()
    {
        MonoManager.Instance.RemoveUpdateEvent(Update);
        StopGenerate();
        Wave = 1;
        Score = 0;
        Remain = 0;
        Death = 0;
        Collection = 0;
        isGenerateBoss = false;
        nowWaveRemainTime = waveTime;
        internalTime = 1f;
    }

    public void Init()
    {
        Clear();
        GameObject map = GameObject.Find("Map");
        SpriteRenderer renderer = map == null ? null : map.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogError("生成敌人需要Map地图");
            return;
        }
        mapBounds = renderer.bounds;
        isRunning = true;
        MonoManager.Instance.AddUpdateEvent(Update);
        UIManager.Instance.ShowPanel<GamePanel>();
        GenerateBoss();
    }

    public void StartNextWave()
    {
        if (isRunning || PlayerInfoManager.Instance.isDead) return;
        nowWaveRemainTime = waveTime;
        internalTime = 1f;
        isGenerateBoss = false;
        isRunning = true;
        Time.timeScale = 1f;
        GenerateBoss();
    }

    private void Update()
    {
        if (!isRunning || Time.timeScale <= 0f) return;
        if (PlayerInfoManager.Instance.isDead || PlayerInfoManager.Instance.playerPos == null)
        {
            StopGenerate();
            return;
        }
        Remain += Time.deltaTime;
        nowWaveRemainTime = Mathf.Max(0f, nowWaveRemainTime - Time.deltaTime);
        if (nowWaveRemainTime <= 0f)
        {
            StopGenerate();
            Wave++;
            UIManager.Instance.ShowPanel<LevelUpPanel>();
            UIManager.Instance.HidePanel<GamePanel>();
            Time.timeScale = 0f;
            internalTime = 1f;
            return;
        }
        internalTime -= Time.deltaTime;
        if (internalTime <= 0)
        {
            GenerateBoss();
            GenerateEnemy();
            internalTime = generateInterval;
        }
    }

    private void GenerateEnemy()
    {
        int count = Mathf.Min(RandomManager.Instance.GetEnemyCount(Wave), maxEnemyCount - Object.FindObjectsOfType<BaseEnemyObject>().Length - pendingCount);
        for (int i = 0; i < count; i++)
        {
            if (TrySpawnPosition(3f, out Vector2 position))
                Generate(RandomManager.Instance.RandomEnemy(Wave), position, false);
        }
    }

    private void GenerateBoss()
    {
        if (Wave <= 0 || Wave % 10 != 0 || isGenerateBoss) return;
        if (Object.FindObjectsOfType<BaseEnemyObject>().Length + pendingCount >= maxEnemyCount) return;
        if (!TrySpawnPosition(5f, out Vector2 position)) return;
        isGenerateBoss = true;
        Generate(RandomManager.Instance.RandomBoss(), position, true);
    }

    private bool TrySpawnPosition(float radius, out Vector2 position)
    {
        for (int i = 0; i < 20; i++)
        {
            if (!RandomManager.Instance.RandomEnemyPosition(mapBounds, radius, out position)) break;
            bool available = true;
            foreach (Vector2 point in spawnPositions)
                if (Vector2.Distance(point, position) < radius * 2f) available = false;
            if (available) return true;
        }
        position = Vector2.zero;
        return false;
    }

    private void Generate(string name, Vector2 position, bool boss)
    {
        int version = spawnVersion;
        int wave = Wave;
        pendingCount++;
        spawnPositions.Add(position);
        PoolManager.Instance.GetObj("EnemySpawn", "eff", obj => {
            GameObject effect = obj as GameObject;
            if (effect == null)
            {
                FinishGenerate(position, version);
                if (version == spawnVersion && boss) isGenerateBoss = false;
                return;
            }
            if (!isRunning || version != spawnVersion)
            {
                PoolManager.Instance.PushObj("EnemySpawn", effect);
                return;
            }
            effect.transform.position = position;
            effect.transform.localScale = Vector3.one * (boss ? 1.8f : 1f);
            effect.SetActive(true);
            effect.GetComponent<EnemySpawnEffect>().Init();
            spawnEffects.Add(effect);
            MonoManager.Instance.StartCoroutine(ReallyGenerate(name, position, boss, effect, wave, version));
        });
    }

    private IEnumerator ReallyGenerate(string name, Vector2 position, bool boss, GameObject effect, int wave, int version)
    {
        yield return new WaitForSeconds(1f);
        if (!isRunning || version != spawnVersion) yield break;
        spawnEffects.Remove(effect);
        PoolManager.Instance.PushObj("EnemySpawn", effect);
        PoolManager.Instance.GetObj(name, "enemy", obj => {
            GameObject enemy = obj as GameObject;
            if (enemy == null)
            {
                FinishGenerate(position, version);
                if (version == spawnVersion && boss) isGenerateBoss = false;
                return;
            }
            if (!isRunning || version != spawnVersion)
            {
                PoolManager.Instance.PushObj(name, enemy);
                return;
            }
            enemy.transform.position = position;
            enemy.SetActive(true);
            BaseEnemyObject data = enemy.GetComponent<BaseEnemyObject>();
            data.isBoss = boss;
            data.Init(wave);
            FinishGenerate(position, version);
        });
    }

    private void FinishGenerate(Vector2 position, int version)
    {
        if (version != spawnVersion) return;
        pendingCount--;
        spawnPositions.Remove(position);
    }

    private void StopGenerate()
    {
        isRunning = false;
        spawnVersion++;
        pendingCount = 0;
        spawnPositions.Clear();
        foreach (var effect in spawnEffects) PoolManager.Instance.PushObj("EnemySpawn", effect);
        spawnEffects.Clear();
        foreach (var enemy in Object.FindObjectsOfType<BaseEnemyObject>())
            PoolManager.Instance.PushObj(enemy.gameObject.name.Replace("(Clone)", "").Trim(), enemy.gameObject);
        foreach (var bullet in Object.FindObjectsOfType<EnemyBullet>()) PoolManager.Instance.PushObj("EnemyBullet", bullet.gameObject);
    }
}
