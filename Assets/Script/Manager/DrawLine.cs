using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();

    public float minDistance = 0.1f; // smoothness control

    private EdgeCollider2D edgeCollider;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        edgeCollider = gameObject.AddComponent<EdgeCollider2D>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartLine();
        }

        if (Input.GetMouseButton(0))
        {
            Draw();
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndLine();
        }
    }

    void StartLine()
    {
        Debug.Log("Start Drawing!");
        points.Clear();
        lineRenderer.positionCount = 0;
        AddPoint(GetMouseWorldPosition());
    }

    void Draw()
    {
        Vector3 mousePos = GetMouseWorldPosition();

        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePos) > minDistance)
        {
            AddPoint(mousePos);
        }
        Debug.Log("Drawing...");
    }

    void EndLine()
    {
        // Optional: Add logic after drawing ends
        Debug.Log("Line Drawn!");
    }

    void AddPoint(Vector3 point)
    {
        points.Add(point);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPosition(points.Count - 1, point);
        edgeCollider.points = points.ConvertAll(p => (Vector2)p).ToArray();
    }

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = 0f; // distance from camera
        return Camera.main.ScreenToWorldPoint(mouse);
    }
}
