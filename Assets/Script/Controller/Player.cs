using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Sprite egg, brokenEgg, chicEgg;
    public bool isGameOver = false;
    public float playerSpeed = 2f;
    public bool isReachedFinish = false;
    //public bool isWaiting = false;

    [HideInInspector] public Rigidbody2D rb;
    private Animator playerAnim;
    private SpriteRenderer sr;
    private bool isGrounded = false;
    private Vector2 moveDirection = Vector2.right;

    [SerializeField] private float checkDistance = 0.5f;
    [SerializeField] private float boxSize = 2f;
    [SerializeField] private Vector2 boxSizeMultiplier = new Vector2(1.5f, 1.5f);
    [SerializeField] private LayerMask lineLayer;
    [SerializeField] private LayerMask wallLayer;

    private Coroutine clearLineRoutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        playerAnim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            AddPlayerMotion();
        }
    }

    void Update()
    {
        //if (isGrounded)
        //{
        //    AddPlayerMotion();
        //}

        TogglePlayerBodyType();
        DestroyPlayer();
    }

    private void ChangeImage()
    {
        if (LevelManager.Instance.currentLevel == 7 && brokenEgg != null)
        {
                sr.sprite = brokenEgg;
        }
    }

    public void WinImage()
    {
        if (LevelManager.Instance.currentLevel == 7 && egg != null)
        {
            sr.sprite = chicEgg;
            sr.transform.localRotation = Quaternion.identity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isGameOver)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Bee") || collision.gameObject.CompareTag("Kite") || collision.gameObject.CompareTag("Spike") 
            || collision.gameObject.CompareTag("Pabble") || collision.gameObject.CompareTag("Chemical")
            )
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");

            //ChangeImage();

            if (collision.gameObject.CompareTag("Bee"))
            {
                Debug.Log("Collided with Bee");
            }
            else if (collision.gameObject.CompareTag("Kite"))
            {
                Debug.Log("Collided with Kite");
            }
            else if (collision.gameObject.CompareTag("Spike"))
            {
                Debug.Log("Collided with Spike");
            }
            else if (collision.gameObject.CompareTag("Pabble"))
            {
                Debug.Log("Collided with Pabble");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameOver)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            isReachedFinish = true;
            Debug.Log("Reached Finish" + isReachedFinish);
        }

        if (collision.gameObject.CompareTag("Chemical") || collision.gameObject.CompareTag("SpiderWeb"))
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");
        }
    }

    private void TogglePlayerBodyType()
    {
        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            if (DrawLineWithMouse.Instance.hasDrawn)
            {
                rb.gravityScale = 1f;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
        if (LevelManager.Instance.currentLevel == 21 && DrawLineWithMouse.Instance.hasDrawn)
        {
            rb.mass = 3f;
            rb.gravityScale = 3f;
            rb.angularVelocity = 0f;
        }
    }

    private void DestroyPlayer()
    {
        float minY = -5.5f, maxY = 5.5f;

        if (isGameOver) return;

        if (gameObject.transform.position.y < minY || gameObject.transform.position.y > maxY)
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");
            //Destroy(gameObject);
        }
    }

    private void AddPlayerMotion()
    {
        if (LevelManager.Instance.currentLevel != 21) return;

        if (IsBlocked() || IsWallAhead())
        {
            ChangeDirection();
        }

        if (!DrawLineWithMouse.Instance.hasDrawn && !DrawLineWithMouse.Instance.isStartedDrawing)
        {
            Motion();
        }
        else if (DrawLineWithMouse.Instance.hasDrawn)
        {
            if (!IsBlocked())
            {
                Motion();
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (SpawnManager.Instance.beeCount != 0 && DrawLineWithMouse.Instance.hasDrawn)
            {
                if (clearLineRoutine == null)
                {
                    clearLineRoutine = StartCoroutine(ClearLineAfterDelay());
                }
            }
        }
    }

    private void Motion()
    {
        rb.linearVelocity = moveDirection * playerSpeed;

        if (moveDirection == Vector2.right)
            rb.transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            rb.transform.rotation = Quaternion.Euler(0, 0, 0);

        rb.freezeRotation = true;
    }

    private void ChangeDirection()
    {
        if (moveDirection == Vector2.right)
            moveDirection = Vector2.left;
        else
            moveDirection = Vector2.right;
    }

    private bool IsWallAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, checkDistance, wallLayer);

        return hit.collider != null;
    }

    private bool IsBlocked()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveDirection, checkDistance, lineLayer);

        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    private bool IsBlockedByObstacle()
    {
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position, 0.5f, Vector2.right, checkDistance, lineLayer);

        if (raycastHit.collider != null)
        {
            return true;
        }

        return false;
    }

    private bool IsPlayerBlocked()
    {
        //Vector2 size = sr.bounds.size * boxSize;
        Vector2 size = Vector2.Scale(sr.bounds.size, boxSizeMultiplier);

        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position,
            size,
            0f,
            Vector3.up,
            checkDistance, 
            lineLayer
        );

        //if (hit.collider != null)
        //{
        //    return true;
        //}

        //return false;
        return hit.collider != null;
    }

    private IEnumerator ClearLineAfterDelay()
    {
        yield return new WaitForSeconds(10f);

        if (DrawLineWithMouse.Instance.hasDrawn)
        {
            DrawLineWithMouse.Instance.ClearLine();
            Debug.Log("CLEARED LINE - 10SEC");

            DrawLineWithMouse.Instance.EnableDrawing();
        }

        clearLineRoutine = null;
    }

    public void StopClearLineTimer()
    {
        if (clearLineRoutine != null)
        {
            StopCoroutine(clearLineRoutine);
            clearLineRoutine = null;
        }
    }

    //RayCast
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)moveDirection * checkDistance);
    }

    //BoxCast
    //private void OnDrawGizmos()
    //{
    //    if (sr == null) return;

    //    Gizmos.color = Color.red;

    //    Vector3 size = Vector2.Scale(sr.bounds.size, boxSizeMultiplier);
    //    Vector3 center = transform.position + Vector3.up * checkDistance;

    //    Gizmos.DrawWireCube(center, size);
    //}

    //CircleCast
    //private void OnDrawGizmos()
    //{
    //    if (sr == null) return;

    //    Gizmos.color = Color.red;
    //    Vector3 center = transform.position + Vector3.right * checkDistance;
    //    Gizmos.DrawSphere(center, 0.5f);
    //}

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
