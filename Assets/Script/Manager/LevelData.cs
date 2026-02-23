using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Player")]
    public Vector3 playerSpawnPosition;

    [Header("Bee Spawn Points")]
    public Vector3[] beeSpawnPositions;

    [Header("Block Spawn Points")]
    public Vector3[] blockSpawnPositions;

    [Header("Level Settings")]
    public int maxBees = 5;
    public float spawnInterval = 0.3f;
}