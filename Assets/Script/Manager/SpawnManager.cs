using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public GameObject beePrefeb,kitePrefab, playerPrefab, pabblePrefab, snakePrefab;
    public Transform spawnPoint, kiteSpawnPoint;
    public Transform[] playerPos;

    public float moveSpeed = 3f;
    public float steerStrength = 2f;
    public float obstacleCheckRadius = 0.1f;

    private List<Rigidbody2D> bees = new List<Rigidbody2D>();
    private List<Rigidbody2D> kites = new List<Rigidbody2D>();
    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> pabbles = new List<GameObject>();
    private GameObject player;
    private Transform playerTransform;
    private int maxBees = 5, count = 0;
    private float spawnInterval = 0.15f, pabbleSpawnInterval = 1.7f, nextSpawnTime, pabbleNextSpawnTime;

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

            if (count == 1)
            {
                SpawnKite();
            }
        }

        if (Time.time >= pabbleNextSpawnTime && DrawLineWithMouse.Instance.hasDrawn)
        {
            SpawnPabbles();
            pabbleNextSpawnTime = Time.time + pabbleSpawnInterval;
        }

        if (playerTransform == null) return;

        StartCoroutine(SnakeScale());
        StartCoroutine(SnakeShrink());
    }

    public void SpawnBee()
    {
        GameObject newBee = Instantiate(beePrefeb, spawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = newBee.GetComponent<Rigidbody2D>();
        bees.Add(rb);
    }

    // Destroy a single bee
    public void DestroyBee(Rigidbody2D beeRb)
    {
        if (beeRb == null) return;

        bees.Remove(beeRb);
        Destroy(beeRb.gameObject);
        count = Mathf.Max(0, count - 1);
    }

    // Destroy ALL bees
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

    public void SpawnKite()
    {
        GameObject newKite = Instantiate(kitePrefab, kiteSpawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = newKite.GetComponent<Rigidbody2D>();
        kites.Add(rb);
    }

    // Destroy a single bee
    public void DestroyKite(Rigidbody2D kiteRb)
    {
        if (kiteRb == null) return;

        kites.Remove(kiteRb);
        Destroy(kiteRb.gameObject);
        count = Mathf.Max(0, count - 1);
    }

    // Destroy ALL bees
    public void DestroyAllKites()
    {
        for (int i = 0; i < kites.Count; i++)
        {
            if (kites[i] != null)
            {
                Destroy(kites[i].gameObject);
            }
        }

        kites.Clear();
        count = 0;
    }

    public void SpawnPabbles()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;

        float leftX = -screenHalfWidth;
        float rightX = screenHalfWidth;

        float spawnPos = Random.Range(leftX, rightX);
        
        GameObject newPabble = Instantiate(pabblePrefab, new Vector3(spawnPos, 7f, 0), Quaternion.identity);
        pabbles.Add(newPabble);
    }

    public void DestroyAllPabbles()
    {
        for (int i = 0; i < pabbles.Count; i++)
        {
            if (pabbles[i] != null)
            {
                Destroy(pabbles[i]);
            }
        }
        pabbles.Clear();
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public void SpawnPlayer()
    {
        Vector3 initialSpawnPos = playerPos[LevelManager.Instance.currentLevel - 1].position;
        player = Instantiate(playerPrefab, initialSpawnPos, Quaternion.identity);
        playerTransform = player.transform;
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

    private IEnumerator SnakeScale()
    {
        //GameObject snake = Instantiate(snakePrefab, new Vector3(1.3f, 1.5f, 0), Quaternion.identity);
        float scaleDuration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            float scale = Mathf.Lerp(2f, 3.5f, elapsedTime / scaleDuration);
            snakePrefab.transform.localScale = new Vector3(2, scale, 2);
            snakePrefab.transform.position = new Vector3(1.35f, 0, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        snakePrefab.transform.localScale = Vector3.one;
    }

    private IEnumerator SnakeShrink()
    {
        float shrinkDuration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < shrinkDuration)
        {
            float scale = Mathf.Lerp(3.5f, 2f, elapsedTime / shrinkDuration);
            snakePrefab.transform.localScale = new Vector3(2, scale, 2);
            snakePrefab.transform.position = new Vector3(1.35f, 1.5f, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        snakePrefab.transform.localScale = Vector3.one;
    }
}
