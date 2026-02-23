using System.Collections;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public static LevelSpawner Instance;

    [Header("Level Data")]
    public LevelData[] levels;
    public int currentLevelIndex = 0;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject beePrefab;
    public GameObject blockPrefab;

    private GameObject player;
    private int beeCount = 0;
    private float nextSpawnTime = 0f;

    private LevelData CurrentLevel => levels[currentLevelIndex];

    private void Awake()
    {
        Instance = this;
    }

    public void StartLevel()
    {
        ClearLevel();
        SpawnPlayer();
        SpawnBlocks();
        beeCount = 0;
        nextSpawnTime = 0f;
    }

    private void Update()
    {
        if (!DrawLineWithMouse.Instance.hasDrawn) return;

        if (Time.time >= nextSpawnTime && beeCount < CurrentLevel.maxBees)
        {
            SpawnBee();
            nextSpawnTime = Time.time + CurrentLevel.spawnInterval;
            beeCount++;
        }
    }

    #region Spawn Methods

    void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, CurrentLevel.playerSpawnPosition, Quaternion.identity);
    }

    void SpawnBlocks()
    {
        if (blockPrefab == null) return;

        foreach (Vector3 pos in CurrentLevel.blockSpawnPositions)
        {
            Instantiate(blockPrefab, pos, Quaternion.identity);
        }
    }

    void SpawnBee()
    {
        if (CurrentLevel.beeSpawnPositions.Length == 0) return;

        // Random spawn point for variation
        int randomIndex = Random.Range(0, CurrentLevel.beeSpawnPositions.Length);
        Vector3 spawnPos = CurrentLevel.beeSpawnPositions[randomIndex];

        Instantiate(beePrefab, spawnPos, Quaternion.identity);
    }

    #endregion

    public Transform GetPlayerTransform()
    {
        if (player == null) return null;
        return player.transform;
    }

    public void ClearLevel()
    {
        // Destroy Player
        if (player != null)
        {
            Destroy(player);
        }

        // Destroy Bees
        GameObject[] bees = GameObject.FindGameObjectsWithTag("Bee");
        foreach (GameObject b in bees)
        {
            Destroy(b);
        }

        // Destroy Blocks
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach (GameObject block in blocks)
        {
            Destroy(block);
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            currentLevelIndex = 0;
        }

        StartLevel();
    }
}