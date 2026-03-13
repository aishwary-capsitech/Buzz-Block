using UnityEngine;

public class Pendulum : MonoBehaviour
{
    public static Pendulum Instance;

    public float maxAngle = 45f;
    public float speed = 2.5f;

    private float swingTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (UIManager.Instance.startPanel.activeSelf)
        {
            ResetPendulum();
        }
        //else if (UIManager.Instance.gameWinPanel.activeSelf || UIManager.Instance.gameOverPanel.activeSelf)
        //{
        //    return;
        //}
        else if (Time.timeScale == 1)
        {
            Swing();
        }
    }

    private void Swing()
    {
        swingTimer += Time.deltaTime;

        float angle = maxAngle * Mathf.Sin(swingTimer * speed);
        transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    public void ResetPendulum()
    {
        swingTimer = 0f;
        transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}