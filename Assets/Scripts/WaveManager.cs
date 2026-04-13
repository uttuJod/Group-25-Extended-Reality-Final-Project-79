using TMPro;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("References")]
    public ZombieSpawner zombieSpawner;
    public TMP_Text waveStatusText;

    [Header("Pickup Spawns")]
    public GameObject ammoPickupPrefab;
    public GameObject healthPickupPrefab;
    public Transform ammoSpawnPoint;
    public Transform healthSpawnPoint;

    [Header("Levels")]
    public LevelConfig[] levels;

    [Header("Start Position")]
    public int startLevelIndex = 0;
    public float timeBetweenWaves = 2f;
    public float incomingWaveTextDuration = 2f;

    int currentLevelIndex;
    int currentWaveIndex;

    bool waiting = false;
    float timer = 0f;

    enum FlowState
    {
        None,
        ShowingIncomingWave,
        BetweenWaves,
        StartingNextLevel,
        AllLevelsComplete
    }

    FlowState state = FlowState.None;

    void Start()
    {
        currentLevelIndex = startLevelIndex;
        currentWaveIndex = 0;

        if (waveStatusText != null)
            waveStatusText.text = "";

        StartCurrentWaveImmediately();
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (!waiting)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            waiting = false;

            if (state == FlowState.ShowingIncomingWave)
            {
                StartCurrentWaveImmediately();
            }
            else if (state == FlowState.BetweenWaves)
            {
                GoToNextWaveOrLevel();
            }
            else if (state == FlowState.StartingNextLevel)
            {
                StartCurrentWaveImmediately();
            }
        }
    }

    void StartCurrentWaveImmediately()
    {
        if (levels == null || levels.Length == 0)
        {
            if (waveStatusText != null)
                waveStatusText.text = "No Levels Configured";
            return;
        }

        if (currentLevelIndex < 0 || currentLevelIndex >= levels.Length)
        {
            if (waveStatusText != null)
                waveStatusText.text = "All Levels Complete!";
            state = FlowState.AllLevelsComplete;
            return;
        }

        LevelConfig level = levels[currentLevelIndex];

        if (level == null || level.waves == null || level.waves.Count == 0)
        {
            if (waveStatusText != null)
                waveStatusText.text = "Level has no waves";
            return;
        }

        if (currentWaveIndex < 0 || currentWaveIndex >= level.waves.Count)
        {
            if (waveStatusText != null)
                waveStatusText.text = "";
            return;
        }

        if (waveStatusText != null)
            waveStatusText.text = "";

        if (zombieSpawner != null)
            zombieSpawner.StartWave(level.waves[currentWaveIndex]);

        state = FlowState.None;
    }

    void GoToNextWaveOrLevel()
    {
        if (levels == null || levels.Length == 0)
            return;

        LevelConfig level = levels[currentLevelIndex];

        currentWaveIndex++;

        if (currentWaveIndex < level.waves.Count)
        {
            if (waveStatusText != null)
                waveStatusText.text = "Incoming Wave";

            waiting = true;
            timer = incomingWaveTextDuration;
            state = FlowState.ShowingIncomingWave;
        }
        else
        {
            currentLevelIndex++;
            currentWaveIndex = 0;

            if (currentLevelIndex >= levels.Length)
            {
                if (waveStatusText != null)
                    waveStatusText.text = "All Levels Complete!";
                state = FlowState.AllLevelsComplete;
                return;
            }

            if (waveStatusText != null)
                waveStatusText.text = "";

            waiting = true;
            timer = timeBetweenWaves;
            state = FlowState.StartingNextLevel;
        }
    }

    void SpawnWaveRewardPickups()
    {
        if (ammoPickupPrefab != null && ammoSpawnPoint != null)
        {
            Instantiate(ammoPickupPrefab, ammoSpawnPoint.position, ammoSpawnPoint.rotation);
        }

        if (healthPickupPrefab != null && healthSpawnPoint != null)
        {
            Instantiate(healthPickupPrefab, healthSpawnPoint.position, healthSpawnPoint.rotation);
        }
    }

    public void OnWaveCompleted()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        SpawnWaveRewardPickups();

        if (waveStatusText != null)
            waveStatusText.text = "";

        waiting = true;
        timer = timeBetweenWaves;
        state = FlowState.BetweenWaves;
    }
}