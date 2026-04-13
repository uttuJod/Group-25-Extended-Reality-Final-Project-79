using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveZombieGroup
{
    public string zombieName;
    public GameObject prefab;
    public int count = 1;
}

[System.Serializable]
public class WaveConfig
{
    public string waveName = "Wave";
    public float warningDuration = 2f;
    public float spawnInterval = 1.5f;
    public int maxAlive = 3;
    public float zombieMoveSpeed = 1f;
    public List<WaveZombieGroup> zombieGroups = new List<WaveZombieGroup>();
}

[System.Serializable]
public class LevelConfig
{
    public string levelName = "Level";
    public List<WaveConfig> waves = new List<WaveConfig>();
}

public class ZombieSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;

    Queue<GameObject> spawnQueue = new Queue<GameObject>();
    float spawnInterval = 1.5f;
    int maxAlive = 3;
    float zombieMoveSpeed = 1f;

    float timer = 0f;
    bool waveActive = false;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (!waveActive)
            return;

        timer += Time.deltaTime;

        if (spawnQueue.Count > 0 && timer >= spawnInterval)
        {
            if (GetAliveZombieCount() < maxAlive)
            {
                timer = 0f;
                SpawnZombie(spawnQueue.Dequeue());
            }
        }

        if (spawnQueue.Count == 0 && GetAliveZombieCount() == 0)
        {
            waveActive = false;

            WaveManager waveManager = FindFirstObjectByType<WaveManager>();
            if (waveManager != null)
                waveManager.OnWaveCompleted();
        }
    }

    public void StartWave(WaveConfig wave)
    {
        spawnQueue.Clear();

        if (wave == null)
            return;

        spawnInterval = wave.spawnInterval;
        maxAlive = wave.maxAlive;
        zombieMoveSpeed = wave.zombieMoveSpeed;
        timer = 0f;
        waveActive = true;

        for (int i = 0; i < wave.zombieGroups.Count; i++)
        {
            WaveZombieGroup group = wave.zombieGroups[i];

            if (group.prefab == null || group.count <= 0)
                continue;

            for (int j = 0; j < group.count; j++)
            {
                spawnQueue.Enqueue(group.prefab);
            }
        }
    }

    void SpawnZombie(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null) return;
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int index = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[index];

        GameObject zombie = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        ZombieAI ai = zombie.GetComponent<ZombieAI>();
        if (ai != null)
            ai.moveSpeed = zombieMoveSpeed;
    }

    int GetAliveZombieCount()
    {
        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Zombie");
        return zombies.Length;
    }
}