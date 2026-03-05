using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public GameObject beePrefeb, crowPrefab, playerPrefab, fishPrefab, eggPrefab, snailPrefab, piranhaPrefab, snakePrefab, circlePrefab, bubblePrefab, tinyFishPrefab, spiderPrefab, webPrefab, beeHouse, slabPrefab, fanPrefab;
    public GameObject[] jellyfishPrefabs;
    public Transform spawnPoint, kiteSpawnPoint, beeHouseSpawnPos, slabPos, fanPos, circlePos, circlePos2, circlePos3;
    public Transform[] playerPos;
    public Transform tinyFishSpawnPos, tinyFishTargetPos, spiderPos;

    public float moveSpeed = 3f;
    public float steerStrength = 2f;
    public float obstacleCheckRadius = 0.1f;

    public bool isAllEnemyDead = false;

    private List<Rigidbody2D> bees = new List<Rigidbody2D>();
    private List<Rigidbody2D> kites = new List<Rigidbody2D>();
    private List<Rigidbody2D> piranha = new List<Rigidbody2D>();
    private List<Rigidbody2D> bubbles = new List<Rigidbody2D>();
    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> pabbles = new List<GameObject>();
    private List<GameObject> circles = new List<GameObject>();
    private List<GameObject> spiders = new List<GameObject>();
    private List<GameObject> webs = new List<GameObject>();
    private List<GameObject> slabs = new List<GameObject>();
    private List<GameObject> fans = new List<GameObject>();
    private GameObject player;
    private Transform playerTransform;
    private int maxBees = 5, count = 0, webCount = 0, slabCount = 0;
    private float spawnInterval = 0.3f, pabbleSpawnInterval = 1.7f, bubbleSpawnInterval = 0.4f, jellySpawnInterval = 3f, tinnyFishSpawnInterval = 10f, webSpawnInterval = 2.5f, nextSpawnTime, pabbleNextSpawnTime, bubbleNextSpawnTime, jellyNextSpawnTime, tinnyFishNextSpawnTime, webNextSpawnTime;
    private Vector3 targetPos;
    private GameObject sharkPrefab, kitePrefab, pabblePrefab;

    private GameObject currentSlab;
    private Rigidbody2D slabRb;
    private Rigidbody2D fanRb;
    private bool slabActivated = false;

    private void Awake()
    {
        Instance = this;

        SetNewTargetPosition();
    }

    private void Update()
    {
        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            CheckFanRb();
        }
    }

    void FixedUpdate()
    {
        if (Time.time >= nextSpawnTime && count != maxBees && DrawLineWithMouse.Instance.hasDrawn)
        {
            SpawnBee();
            SpawnPiranha();
            nextSpawnTime = Time.time + spawnInterval;
            count++;

            if (count == 1)
            {
                SpawnSpider();
                SpawnKite();
            }
        }

        if (Time.time >= jellyNextSpawnTime && !UIManager.Instance.startPanel.activeSelf)
        {
            SpawnJelly();
            jellyNextSpawnTime = Time.time + jellySpawnInterval;
        }

        if (Time.time >= bubbleNextSpawnTime && !UIManager.Instance.startPanel.activeSelf)
        {
            SpawnBubbles();
            bubbleNextSpawnTime = Time.time + bubbleSpawnInterval;
        }

        if (Time.time >= tinnyFishNextSpawnTime && !UIManager.Instance.startPanel.activeSelf)
        {
            SpawnTinyFish();
            tinnyFishNextSpawnTime = Time.time + tinnyFishSpawnInterval;
        }

        if (Time.time >= webNextSpawnTime && DrawLineWithMouse.Instance.hasDrawn)
        {
            SpawnSpiderWeb();
            webNextSpawnTime = Time.time + webSpawnInterval;
            webCount++;
        }

        if (currentSlab == null) return;

        if (DrawLineWithMouse.Instance.hasDrawn && !slabActivated)
        {
            slabRb.bodyType = RigidbodyType2D.Dynamic;
            slabRb.gravityScale = 0.7f;
            slabActivated = true;
        }

        if (Time.time >= nextSpawnTime && slabCount != 1)
        {
            slabCount++;
            if(slabCount == 1)
                SpawnSlab();
        }

        if (playerTransform == null) return;

        if (LevelManager.Instance.currentLevel == 0)
            MovePlayerSmoothly();

        StartCoroutine(SnakeScale());
        StartCoroutine(SnakeShrink());

        //if (Time.time >= pabbleNextSpawnTime && DrawLineWithMouse.Instance.hasDrawn)
        //{
        //    SpawnPabbles();
        //    pabbleNextSpawnTime = Time.time + pabbleSpawnInterval;
        //}
    }

    public void SpawnBubbles()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenHalfHeight = Camera.main.orthographicSize;

        float leftX = -screenHalfWidth;
        float rightX = screenHalfWidth;
        float topY = screenHalfHeight;
        float bottomY = -screenHalfHeight;

        float spawnPosX = Random.Range(leftX, rightX);
        float spawnPosY = Random.Range(bottomY, topY);

        float minorSpawnPosX = Random.Range(0.25f, 1.7f);
        float minorSpawnPosY = Random.Range(-4.8f, -4f);

        float spawnPosXLvl9 = Random.Range(-1.8f, -0.3f);
        float spawnPosYLvl9 = Random.Range(-5.7f, -3.5f);

        float spawnPosXLvl13 = Random.Range(-1.25f, 2f);
        float spawnPosYLvl13 = Random.Range(-5.4f, -3f);

        float scale = 0;
        if (LevelManager.Instance.currentLevel == 0)
        {
            scale = Random.Range(0.15f, 0.5f);
        }
        else if (LevelManager.Instance.currentLevel == 8 || LevelManager.Instance.currentLevel == 9 || LevelManager.Instance.currentLevel == 13)
        {
            scale = Random.Range(0.15f, 0.3f);
        }
        bubblePrefab.transform.localScale = new Vector3(scale, scale, 1f);

        string[] hexColors = { "#589FDB", "#9AB9D2", "#988DE5" };
        Color[] colors = new Color[hexColors.Length];
        for (int i = 0; i < hexColors.Length; i++)
        {
            ColorUtility.TryParseHtmlString(hexColors[i], out colors[i]);
        }

        GameObject newBubble = null;

        if (LevelManager.Instance.currentLevel == 0)
        {
            newBubble = Instantiate(bubblePrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity);
            newBubble.GetComponent<SpriteRenderer>().color = colors[0];
        }
        else
        {
            if (LevelManager.Instance.currentLevel == 8)
            {
                newBubble = Instantiate(bubblePrefab, new Vector3(minorSpawnPosX, minorSpawnPosY, 0f), Quaternion.identity);
                newBubble.GetComponent<SpriteRenderer>().color = colors[1];
            }
            if (LevelManager.Instance.currentLevel == 9)
            {
                newBubble = Instantiate(bubblePrefab, new Vector3(spawnPosXLvl9, spawnPosYLvl9, 0f), Quaternion.identity);
                newBubble.GetComponent<SpriteRenderer>().color = colors[1];
            }
            if (LevelManager.Instance.currentLevel == 13)
            {
                newBubble = Instantiate(bubblePrefab, new Vector3(spawnPosXLvl13, spawnPosYLvl13, 0f), Quaternion.identity);
                newBubble.GetComponent <SpriteRenderer>().color = colors[2];
            }
        }

        if(newBubble != null)
        {
            Rigidbody2D bubbleNewRb = newBubble.GetComponent<Rigidbody2D>(); 
            bubbles.Add(bubbleNewRb);
        }
    }

    public void DestroyBubble(Rigidbody2D bubbleRb)
    {
        if (bubbleRb == null) return;

        bubbles.Remove(bubbleRb);
        Destroy(bubbleRb.gameObject);
    }

    public void DestroyAllBubbles()
    {
        for (int i = 0;i < bubbles.Count;i++)
        {
            if (bubbles[i] != null)
            {
                Destroy(bubbles[i].gameObject);
            }
        }
        bubbles.Clear();
    }

    public void SpawnJelly()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenHalfHeight = Camera.main.orthographicSize;
        float leftX = -screenHalfWidth;
        float rightX = screenHalfWidth;
        float topY = screenHalfHeight;
        float bottomY = -screenHalfHeight;

        float spawnPosX = Random.Range(leftX, rightX);
        float spawnPosY = Random.Range(bottomY, topY);

        float scale = Random.Range(0.1f, 0.3f);
        float scale2 = Random.Range(0.05f, 0.1f);

        int index = Random.Range(0, jellyfishPrefabs.Length);

        if (LevelManager.Instance.currentLevel != 0)
        {
            return;
        }

        GameObject newJelly = Instantiate(jellyfishPrefabs[index], new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity);

        if (index == 0)
        {
            newJelly.transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            newJelly.transform.localScale = new Vector3(scale2, scale2, 1f);
        }
    }

    public void SpawnTinyFish()
    {
        if (LevelManager.Instance.currentLevel != 0)
        {
            return;
        }

        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenHalfHeight = Camera.main.orthographicSize;

        float leftX = -screenHalfWidth;
        float rightX = screenHalfWidth;
        float topY = screenHalfHeight;
        float bottomY = -screenHalfHeight;

        float[] xPos = { leftX, rightX };
        int index = Random.Range(0, xPos.Length);

        float randomX = xPos[index];
        float randomY = Random.Range(topY, bottomY);
        tinyFishSpawnPos.position = new Vector3(randomX, randomY, 0f);

        float rotation = 0f;

        if (randomX == rightX)
        {
            rotation = 180f;
            tinyFishTargetPos.position = new Vector3(leftX - 5f, randomY, 0f);
        }
        else
        {
            rotation = 0f;
            tinyFishTargetPos.position = new Vector3(rightX + 5f, randomY, 0f);
        }

        float randomGravity = Random.Range(-0.05f, 0f);

        for (int i = 0; i < 8; i++)
        {
            float spawnPosX = Random.Range(tinyFishSpawnPos.position.x - 1f, tinyFishSpawnPos.position.x + 1f);
            float spawnPosY = Random.Range(tinyFishSpawnPos.position.y - 1f, tinyFishSpawnPos.position.y + 1f);
            GameObject newTinyFish = Instantiate(tinyFishPrefab, new Vector3(spawnPosX, spawnPosY, 0f), Quaternion.identity);
            Rigidbody2D rb = newTinyFish.GetComponent<Rigidbody2D>();
            Vector2 direction = (tinyFishTargetPos.position - newTinyFish.transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed;
            rb.gravityScale = randomGravity;
            newTinyFish.transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }
    }

    public void SpawnPiranha()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenHalfHeight = Camera.main.orthographicSize;

        float leftX = -screenHalfWidth;
        float rightX = screenHalfWidth;
        float topY = screenHalfHeight;
        float bottomY = -screenHalfHeight;

        float[] randomX = { leftX, rightX };
        float[] randomY = { bottomY, topY };

        int indexX = Random.Range(0, randomX.Length);
        int indexY = Random.Range(0, randomY.Length);

        GameObject newPiranha;

        if (LevelManager.Instance.currentLevel == 0)
        {
            newPiranha = Instantiate(piranhaPrefab, new Vector3(randomX[indexX], randomY[indexY], 0f), Quaternion.identity);
        }
        else
        {
            return;
        }
        Rigidbody2D rb = newPiranha.GetComponent<Rigidbody2D>();
        piranha.Add(rb);
    }

    public void DestroyAllPiranha()
    {
        for (int i = 0; i < piranha.Count; i++)
        {
            if (piranha[i] != null)
            {
                Destroy(piranha[i].gameObject);
            }
        }
        piranha.Clear();
        count = 0;
    }

    public void SpawnBee()
    {
        GameObject newBee;
        //newBee = Instantiate(beePrefeb, spawnPoint.position, Quaternion.identity);
        if (LevelManager.Instance.currentLevel == 0)
        {
            //newBee = Instantiate(piranhaPrefab, spawnPoint.position, Quaternion.identity);
            return;
        }
        if (LevelManager.Instance.currentLevel == 7 || LevelManager.Instance.currentLevel == 8)
        {
            newBee = Instantiate(crowPrefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            newBee = Instantiate(beePrefeb, spawnPoint.position, Quaternion.identity);
        }
        Rigidbody2D rb = newBee.GetComponent<Rigidbody2D>();
        bees.Add(rb);
    }

    public void SpawnBeesInBunch()
    {
        for (int i = 0; i < maxBees; i++)
        {
            SpawnBee();
        }
    }

    // Destroy a single bee
    public void DestroyBee(Rigidbody2D beeRb)
    {
        if (beeRb == null) return;

        bees.Remove(beeRb);
        Destroy(beeRb.gameObject);
        //count = Mathf.Max(0, count - 1);

        CheckIsEnemiesDead();
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

    public void SpawnShark()
    {
        GameObject newShark = Instantiate(sharkPrefab, spawnPoint.position, Quaternion.identity);
        Rigidbody2D rb = newShark.GetComponent<Rigidbody2D>();
        bees.Add(rb);
    }

    public void SpawnKite()
    {
        GameObject newKite = null;
        //newKite = Instantiate(kitePrefab, kiteSpawnPoint.position, Quaternion.identity);

        if (newKite != null)
        {
            if (LevelManager.Instance.currentLevel == 0)
            {
                newKite = Instantiate(sharkPrefab, kiteSpawnPoint.position, Quaternion.identity);
            }
            else
            {
                newKite = Instantiate(kitePrefab, kiteSpawnPoint.position, Quaternion.identity);
            }

            Rigidbody2D rb = newKite.GetComponent<Rigidbody2D>();
            kites.Add(rb);
        }
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

    public void SpawnSpider()
    {
        GameObject spider = null;

        if (LevelManager.Instance.currentLevel == 11)
        {
            spider = Instantiate(spiderPrefab, spiderPos.position, Quaternion.identity);
        }

        spiders.Add(spider);
    }

    public void DestroyAllSpiders()
    {
        for (int i = 0;i < spiders.Count;i++)
        {
            if (spiders[i] != null)
            {
                Destroy(spiders[i]);
            }
        }
        spiders.Clear();
    }

    public void SpawnSpiderWeb()
    {
        GameObject web = null;

        if (LevelManager.Instance.currentLevel == 11)
        {
            web = Instantiate(webPrefab, spiderPos.position, Quaternion.identity);
        }

        webs.Add(web);
    }

    public void DestroyAllSpiderWebs()
    {
        for (int i = 0; i < webs.Count; i++)
        {
            if (webs[i] != null)
            {
                Destroy(webs[i]);
            }
        }
        webs.Clear();
        webCount = 0;
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
        Vector3 initialSpawnPos = playerPos[LevelManager.Instance.currentLevel].position;
        if (LevelManager.Instance.currentLevel == 0)
        {
            player = Instantiate(fishPrefab, initialSpawnPos, Quaternion.identity);
        }
        else if (LevelManager.Instance.currentLevel == 7)
        {
            player = Instantiate(eggPrefab, initialSpawnPos, Quaternion.identity);
        }
        else if (LevelManager.Instance.currentLevel >= 8)
        {
            player = Instantiate(snailPrefab, initialSpawnPos, Quaternion.identity);
        }
        else
        {
            player = Instantiate(playerPrefab, initialSpawnPos, Quaternion.identity);
        }
        //player = Instantiate(playerPrefab, initialSpawnPos, Quaternion.identity);
        playerTransform = player.transform;
        players.Add(player);
    }

    void MovePlayerSmoothly()
    {
        Vector3 currentPos = player.transform.position;

        Vector3 scale = player.transform.localScale;

        if (targetPos.x > currentPos.x)
        {
            scale.x = -Mathf.Abs(scale.x);
        }
        else if (targetPos.x < currentPos.x)
        {
            scale.x = Mathf.Abs(scale.x);
        }

        Vector3 dir = (targetPos - currentPos).normalized;
        float playerDir = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (targetPos.x < currentPos.x)
        {
            playerDir += 180f;
        }
        player.transform.rotation = Quaternion.Euler(0, 0, playerDir);


        player.transform.localScale = scale;

        player.transform.position = Vector3.MoveTowards(
            player.transform.position,
            targetPos,
            (moveSpeed - 1f) * Time.deltaTime
        );

        if (Vector3.Distance(player.transform.position, targetPos) < 0.05f)
        {
            SetNewTargetPosition();
        }
    }

    void SetNewTargetPosition()
    {
        float leftX = -4.5f;
        float rightX = 4.5f;
        float topY = 1.5f;
        float bottomY = -1.5f;

        float randomX = Random.Range(leftX, rightX);
        float randomY = Random.Range(bottomY, topY);

        targetPos = new Vector3(randomX, randomY, 0f);
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

    public void SpawnCircle()
    {
        Transform spawnPos = null;
        if (LevelManager.Instance.currentLevel == 7)
        {
            spawnPos = circlePos;
        }
        else if (LevelManager.Instance.currentLevel == 9)
        {
            spawnPos = circlePos2;
        }
        else if (LevelManager.Instance.currentLevel == 10)
        {
            spawnPos = circlePos3;
        }
        GameObject newCircle = Instantiate(circlePrefab, spawnPos.position, Quaternion.identity);
        circles.Add(newCircle);
    }

    public void DestroyAllCircles()
    {
        for (int i = 0; i < circles.Count; i++)
        {
            if (circles[i] != null)
            {
                Destroy(circles[i]);
            }
        }
        circles.Clear();
    }

    private void CheckIsEnemiesDead()
    {
        if (!DrawLineWithMouse.Instance.hasDrawn) return;

        if (bees.Count == 0)
        {
            isAllEnemyDead = true;
        }
    }

    public void SpawnSlab()
    {
        if (currentSlab != null)
        {
            Destroy(currentSlab);
        }

        currentSlab = Instantiate(slabPrefab, slabPos.position, Quaternion.Euler(-60f, -30f, 0f));

        slabRb = currentSlab.GetComponent<Rigidbody2D>();

        slabRb.linearVelocity = Vector2.zero;
        slabRb.angularVelocity = 0f;
        slabRb.bodyType = RigidbodyType2D.Kinematic;

        slabActivated = false;
    }

    public void DestroyAllSlabs()
    {
        for (int i = 0; i < slabs.Count; i++)
        {
            if(slabs[i] != null)
            {
                Destroy(slabs[i]);
                slabActivated = false;
            }
        }
        slabs.Clear();
    }

    public void SpawnFan()
    {
        if (fanPrefab == null)
        {
            Debug.Log("FanPrefab Null");
            return;
        }

        GameObject fan = null;

        if (fan == null)
        {
            Debug.Log("Fan Null");
        }
        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            fan = Instantiate(fanPrefab, fanPos.position, Quaternion.identity);
            Debug.Log("Fan Spawned");
        }
        fanRb = fan.GetComponent<Rigidbody2D>();
        fans.Add(fan);
    }

    void CheckFanRb()
    {
        if (fanRb == null) return;

        if (DrawLineWithMouse.Instance.hasDrawn)
        {
            fanRb.gravityScale = -2f;
        }
        else
        {
            fanRb.gravityScale = 0f;
        }
    }

    public void DestroyAllFans()
    {
        for (int i = 0;i < fans.Count; i++)
        {
            if (fans[i] != null)
            {
                Destroy(fans[i]);
            }
        }
        //Destroy(fanRb);
        fans.Clear();
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
