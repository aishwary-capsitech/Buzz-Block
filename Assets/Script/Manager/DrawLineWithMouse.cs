using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class DrawLineWithMouse : MonoBehaviour
{
    public static DrawLineWithMouse Instance;

    public float minDistance = 0.1f;
    public EdgeCollider2D edgeCollider;
    public bool hasDrawn = false;
    public bool canDraw = false;
    public bool isStartedDrawing = false;

    private LineRenderer lineRenderer;
    [HideInInspector] public List<Vector2> localPoints = new List<Vector2>();
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

    //void Start()
    //{
    //    initialPosition = transform.position;
    //    lineRenderer = GetComponent<LineRenderer>();
    //    edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
    //    edgeCollider.enabled = false;
    //    edgeCollider.edgeRadius = 0.05f;

    //    rb = GetComponent<Rigidbody2D>();
    //    rb.linearVelocity = Vector2.zero;
    //    rb.angularVelocity = 0f;
    //    AddKinematic();
    //}

    void Start()
    {
        initialPosition = transform.position;

        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
        edgeCollider.enabled = false;
        edgeCollider.edgeRadius = 0.05f;

        rb = GetComponent<Rigidbody2D>();

        // Ignore collision with player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Physics2D.IgnoreCollision(edgeCollider, player.GetComponent<Collider2D>());
        }

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
        isStartedDrawing = true;
    }

    void DrawLine()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;

        //Collider2D hit = Physics2D.OverlapPoint(worldPos);
        //if (hit != null)
        //{
        //    return;
        //}

        Vector2 wordPoint2D = worldPos;
        if (localPoints.Count > 0)
        {
            Vector2 previousWorld = transform.TransformPoint(previousPoint);

            //RaycastHit2D hit = Physics2D.Linecast(previousWorld, wordPoint2D);
            //if (hit.collider != null)
            //{
            //    return;
            //}

            RaycastHit2D hit = Physics2D.Linecast(previousWorld, wordPoint2D);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                    return;

                if (!hit.collider.isTrigger)
                    return;
            }
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

    void UpdateCenterOfMass()
    {
        if (localPoints.Count == 0) return;

        Vector2 center = Vector2.zero;

        for (int i = 0; i < localPoints.Count; i++)
        {
            center += localPoints[i];
        }

        center /= localPoints.Count;

        rb.centerOfMass = center;
    }

    void StopDrawingAndEnableGravity()
    {
        if(localPoints.Count < 2)
        {
            return;
        }

        hasDrawn = true;

        edgeCollider.enabled = true;
        UpdateCenterOfMass();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 2f;

        int currentLevel = LevelManager.Instance.currentLevel;
        if (currentLevel == 7 || currentLevel == 9 || currentLevel == 10 || currentLevel == 18)
        {
            SpawnManager.Instance.SpawnCircle();
        }

        //if (currentLevel == 15)
        //{
        //    rb.mass = 2f;
        //}
        //else
        //{
        //    rb.mass = 10f;
        //}
    }

    public void ClearLine()
    {
        transform.position = initialPosition;
        transform.rotation = Quaternion.identity;

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.centerOfMass = Vector2.zero;
        rb.Sleep();

        edgeCollider.enabled = false;

        localPoints.Clear();
        lineRenderer.positionCount = 0;
        edgeCollider.points = new Vector2[0];

        hasDrawn = false;
        canDraw = false;

        if (LevelManager.Instance.currentLevel == 21)
            isStartedDrawing = false;
    }

    public void EnableDrawing()
    {
        localPoints.Clear();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.07f;

        canDraw = true;
    }

    public IEnumerator ResetLineAfterEnemiesDead()
    {
        yield return new WaitForSeconds(1f);

        ClearLine();
        Debug.Log("CLEARED LINE - 1SEC");
        EnableDrawing();
    }
}