using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class DrawLineWithMouse : MonoBehaviour
{
    public static DrawLineWithMouse Instance;

    public float minDistance = 0.1f;
    public EdgeCollider2D edgeCollider;
    public bool hasDrawn = false;
    public bool canDraw = false;

    private LineRenderer lineRenderer;
    private List<Vector2> localPoints = new List<Vector2>();
    private Rigidbody2D rb;
    private Vector2 initialPosition;
    private Vector2 previousPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        initialPosition = transform.position;
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.edgeRadius = 0.05f;

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        AddKinematic();
    }

    void Update()
    {
        if (hasDrawn || !canDraw) return;

        if (Input.GetMouseButtonDown(0))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = 0f;
            StartDrawing();
        }

        if (Input.GetMouseButton(0))
        {
            DrawLine();
        }

        if (Input.GetMouseButtonUp(0))
        {
            StopDrawingAndEnableGravity();
        }
    }

    public void AddKinematic()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    public void StartDrawing()
    {
        AddKinematic();

        edgeCollider.enabled = false;
        localPoints.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.07f;
    }

    void DrawLine()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            return;
        }

        Vector2 localPos = transform.InverseTransformPoint(worldPos);

        if (localPoints.Count == 0 || Vector2.Distance(localPos, previousPoint) > minDistance)
        {
            localPoints.Add(localPos);
            lineRenderer.positionCount = localPoints.Count;

            for (int i = 0; i < localPoints.Count; i++)
            {
                lineRenderer.SetPosition(i, localPoints[i]);
            }

            edgeCollider.points = localPoints.ToArray();
            previousPoint = localPos;
        }
    }

    void StopDrawingAndEnableGravity()
    {
        hasDrawn = true;

        edgeCollider.enabled = true;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
    }

    public void ClearLine()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.Sleep();

        edgeCollider.enabled = false;

        localPoints.Clear();
        lineRenderer.positionCount = 0;
        edgeCollider.points = new Vector2[0];

        hasDrawn = false;
        canDraw = false;
    }

    public void EnableDrawing()
    {
        localPoints.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.07f;

        canDraw = true;
    }
}