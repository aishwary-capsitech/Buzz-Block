using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Bee : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isStunned = false;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Stun Settings")]
    public float stunDuration = 0.2f;
    public float stunToPlayer = 0.07f;
    public float stunToBee = 0.03f;

    private Transform player;

    void Awake()
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
        if (isStunned || player == null) return;

        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        rb.linearVelocity = dir * moveSpeed;

        ChangeDir();
    }

    private void ChangeDir()
    {
        if (gameObject.transform.position.x < player.position.x)
        {
            transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider == DrawLineWithMouse.Instance.edgeCollider)
        {
            StartCoroutine(StunCoroutine(stunDuration));
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(StunCoroutine(stunToPlayer));
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            StartCoroutine(StunCoroutine(stunDuration));
        }

        if (collision.gameObject.CompareTag("Spike"))
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator StunCoroutine(float duration)
    {
        if (isStunned) yield break;

        isStunned = true;

        // Stop movement instantly
        rb.linearVelocity = -rb.linearVelocity * moveSpeed;

        // Wait stun time
        yield return new WaitForSeconds(duration);

        isStunned = false;
    }
}
