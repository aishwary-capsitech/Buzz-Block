using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public GameObject beePrefeb, playerPrefab;
    public Transform spawnPoint;
    public Transform[] playerPos;

    private List<Rigidbody2D> bees = new List<Rigidbody2D>();
    private List<GameObject> players = new List<GameObject>();
    private Vector3 catPos;
    private int maxBees = 5, count = 0;
    private float spawnInterval = 1.5f, nextSpawnTime;

    public float moveSpeed = 3f;
    public float steerStrength = 2f;
    public float obstacleCheckRadius = 0.1f;

    private void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        if (Time.time >= nextSpawnTime && count != maxBees && DrawLineWithMouse.Instance.hasDrawn)
        {
            SpawnBee();
            nextSpawnTime = Time.time + spawnInterval;
            count++;
        }

        for (int i = 0; i < bees.Count; i++)
        {
            Rigidbody2D b = bees[i];

            if (b == null) continue;

            Vector2 beePos = b.position;
            Vector2 targetPos = catPos;

            Vector2 toPlayer = (targetPos - beePos).normalized;

            EdgeCollider2D lineCollider = DrawLineWithMouse.Instance.edgeCollider;
            bool pathBlocked = false;

            if (lineCollider != null && lineCollider.pointCount > 1)
            {
                RaycastHit2D hit = Physics2D.Linecast(beePos, targetPos);

                if (hit.collider == lineCollider)
                {
                    pathBlocked = true;
                }
            }

            Vector2 moveDir;

            if (!pathBlocked)
            {
                moveDir = toPlayer;
            }
            else
            {
                // Slide along line instead of pushing upward
                Vector2 slideDir = new Vector2(toPlayer.y, -toPlayer.x);
                moveDir = (toPlayer * 0.7f + slideDir * 0.3f).normalized;
            }

            b.MovePosition(beePos + moveDir * moveSpeed * Time.fixedDeltaTime);
        }
    }

    public void SpawnBee()
    {
        GameObject newBee = Instantiate(beePrefeb, spawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = newBee.GetComponent<Rigidbody2D>();
        bees.Add(rb);
    }

    // Destroy a single bee (can be called when bee hits player, etc.)
    public void DestroyBee(Rigidbody2D beeRb)
    {
        if (beeRb == null) return;

        bees.Remove(beeRb);
        Destroy(beeRb.gameObject);
        count = Mathf.Max(0, count - 1);
    }

    // Destroy ALL bees (call on level end, game over, reset, etc.)
    public void DestroyAllBees()
    {
        for (int i = 0; i < bees.Count; i++)
        {
            if (bees[i] != null)
            {
                Destroy(bees[i].gameObject);
            }
        }

        bees.Clear();
        count = 0;
    }

    public void ResetSpawner()
    {
        nextSpawnTime = 0f;
        count = 0;
    }

    public void SpawnPlayer()
    {
        catPos = playerPos[LevelManager.Instance.currentLevel - 1].position;
        GameObject player = Instantiate(playerPrefab, catPos, Quaternion.identity);
        players.Add(player);
    }

    public void DestroyPlayer()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                Destroy(players[i]);
            }
        }

        players.Clear();
    }
}
