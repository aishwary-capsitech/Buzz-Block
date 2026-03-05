using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (SpawnManager.Instance != null)
        {
            player = SpawnManager.Instance.GetPlayerTransform();
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("LineDrawer"))
        {
            Destroy(gameObject);
        }
    }
}
