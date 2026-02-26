using UnityEngine;

public class CircularSpike : MonoBehaviour
{
    public float speed = 2.5f;
    public float distance = 7.2f;
    public float rotationSpeed = 360f;

    private Vector3 startPos;
    private Vector3 lastPosition;
    private int level;
    private float currentRotationDir = 1f;

    private void Start()
    {
        level = LevelManager.Instance.currentLevel;
        startPos = transform.position;
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (level != 10) return;

        Move();
        HandleRotationDirection();
        RotateSpike();

        lastPosition = transform.position;
    }

    private void Move()
    {
        float movement = Mathf.PingPong(Time.time * speed, distance);
        transform.position = startPos + Vector3.right * movement;
    }

    private void HandleRotationDirection()
    {
        float deltaX = transform.position.x - lastPosition.x;

        if (deltaX > 0.001f)
        {
            currentRotationDir = 1f;
        }
        else if (deltaX < -0.001f)
        {
            currentRotationDir = -1f;
        }
    }

    private void RotateSpike()
    {
        transform.Rotate(0f, 0f, -rotationSpeed * currentRotationDir * Time.deltaTime);
    }
}