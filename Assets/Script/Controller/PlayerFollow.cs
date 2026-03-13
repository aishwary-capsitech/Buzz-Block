using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public static PlayerFollow instance;

    Vector3 currentPos;
    Vector3 newPos;

    private Transform target;
    private float followSpeed = 5f;
    private float screenTop;
    public float yOffset = 1.5f;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (SpawnManager.Instance.player == null) return;

        target = SpawnManager.Instance.player.transform;
        screenTop = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        CameraSize();
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (LevelManager.Instance.currentLevel == 21)
            FollowPlayer();
    }

    void FollowPlayer()
    {
        currentPos = transform.position;
        newPos = currentPos;

        // Follow only X
        newPos.x = Mathf.Lerp(currentPos.x, target.position.x, followSpeed * Time.deltaTime);

        // Keep Y fixed (or smooth to a fixed value)
        newPos.y = Mathf.Lerp(currentPos.y, target.position.y + yOffset, followSpeed * Time.deltaTime);

        transform.position = new Vector3(newPos.x, newPos.y, -10);
    }

    void CameraSize()
    {
        if (LevelManager.Instance.currentLevel == 21)
        {
            Camera.main.orthographicSize = 3;
        }
        else
        {
            Camera.main.orthographicSize = 5;
        }
    }
}