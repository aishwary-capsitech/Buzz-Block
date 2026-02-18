using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject startPanel, gameWinPanel, gameOverPanel, gamePlayPanel;
    public Image timerImage;

    [SerializeField] private DrawLineWithMouse drawLine;
    private LineRenderer lr;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        lr = drawLine.GetComponent<LineRenderer>();

        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);

        lr.enabled = false;
        drawLine.canDraw = false;
    }

    private void FixedUpdate()
    {
        if (drawLine.hasDrawn)
        {
            UpdateTimer();
        }

        if (timerImage != null && timerImage.fillAmount <= 0)
        {
            GameWin();
        }
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        lr.enabled = true;

        StartCoroutine(EnableDrawingNextFrame());
    }

    private IEnumerator EnableDrawingNextFrame()
    {
        yield return null;
        drawLine.EnableDrawing();
    }

    private void UpdateTimer()
    {
        if (timerImage != null && !Player.Instance.isGameOver)
        {
            timerImage.fillAmount -= Time.deltaTime * 0.15f;
        }
    }

    private void ResetTimer()
    {
        if (timerImage != null)
        {
            timerImage.fillAmount = 1f;
        }
    }

    public void GameWin()
    {
        gameWinPanel.SetActive(true);
        DrawLineWithMouse.Instance.AddKinematic();
    }

    public void GameOver()
    {
        if(timerImage.fillAmount > 0)
        {
            gameOverPanel.SetActive(true);
            gameWinPanel.SetActive(false);
            DrawLineWithMouse.Instance.AddKinematic();
        }
    }

    public void RetryGame()
    {
        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.ResetSpawner();

        if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);

        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);

        ResetTimer();
        Player.Instance.isGameOver = false;

        drawLine.ClearLine();
        lr.enabled = true;

        StartCoroutine(EnableDrawingNextFrame());
    }

    public void NextGame()
    {
        LevelManager.Instance.IncreaseLevel();
        LevelManager.Instance.DisplayLevel();

        SpawnManager.Instance.DestroyPlayer();
        SpawnManager.Instance.SpawnPlayer();

        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.ResetSpawner();

        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);

        ResetTimer();
        Player.Instance.isGameOver = false;
        drawLine.ClearLine();
        lr.enabled = true;
        StartCoroutine(EnableDrawingNextFrame());
    }
}