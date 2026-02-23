using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject startPanel, gameWinPanel, gameOverPanel, gamePlayPanel, nextButton;
    public Image timerImage;
    public TextMeshProUGUI levelText;

    [SerializeField] private DrawLineWithMouse drawLine;
    private LineRenderer lr;
    private bool isGameRunning = false;

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
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);

        lr.enabled = false;
        drawLine.canDraw = false;
    }

    private void FixedUpdate()
    {
        if (!isGameRunning)
        {
            return;
        }

        if (drawLine.hasDrawn && !Player.Instance.isGameOver && !gameWinPanel.activeSelf)
        {
            UpdateTimer();
        }

        if (timerImage != null && timerImage.fillAmount <= 0)
        {
            GameWin();
        }

        DisplayLevel();
    }

    private void DisplayLevel()
    {
        if (levelText != null)
        {
            levelText.text = "Level " + LevelManager.Instance.currentLevel;
        }

        if (LevelManager.Instance.currentLevel == 5)
        {
            nextButton.SetActive(false);
        }
    }

    public void StartGame()
    {
        isGameRunning = true;
        gamePlayPanel.SetActive(true);
        LevelManager.Instance.DisplayLevel();
        SpawnManager.Instance.SpawnPlayer();
        startPanel.SetActive(false);
        lr.enabled = true;
        nextButton.SetActive(true);

        StartCoroutine(EnableDrawingNextFrame());
    }

    private IEnumerator EnableDrawingNextFrame()
    {
        yield return null;
        drawLine.EnableDrawing();
    }

    private void UpdateTimer()
    {
        if (timerImage != null)
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
        isGameRunning = false;
        gameWinPanel.SetActive(true);
        //DrawLineWithMouse.Instance.AddKinematic();
    }

    public void GameOver()
    {
        #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
        #endif

        if (timerImage.fillAmount > 0)
        {
            isGameRunning = false;
            gameOverPanel.SetActive(true);
            gameWinPanel.SetActive(false);
            //DrawLineWithMouse.Instance.AddKinematic();
        }
    }

    public void BackHome()
    {
        DrawLineWithMouse.Instance.AddKinematic();
        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.DestroyAllKites();
        SpawnManager.Instance.DestroyAllPabbles();
        //SpawnManager.Instance.ResetSpawner();
        SpawnManager.Instance.DestroyPlayer();
        if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);
        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);
        ResetTimer();
        Player.Instance.isGameOver = false;
        drawLine.ClearLine();
        lr.enabled = false;
        startPanel.SetActive(true);
        LevelManager.Instance.currentLevel = LevelManager.Instance.startLevel;
    }

    public void RetryGame()
    {
        isGameRunning = true;
        DrawLineWithMouse.Instance.AddKinematic();
        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.DestroyAllKites();
        SpawnManager.Instance.DestroyAllPabbles();
        //SpawnManager.Instance.ResetSpawner();

        SpawnManager.Instance.DestroyPlayer();
        SpawnManager.Instance.SpawnPlayer();

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
        isGameRunning = true;
        DrawLineWithMouse.Instance.AddKinematic();

        LevelManager.Instance.IncreaseLevel();
        LevelManager.Instance.DisplayLevel();

        SpawnManager.Instance.DestroyPlayer();
        SpawnManager.Instance.SpawnPlayer();

        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.DestroyAllKites();
        SpawnManager.Instance.DestroyAllPabbles();
        //SpawnManager.Instance.ResetSpawner();

        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);

        ResetTimer();
        Player.Instance.isGameOver = false;
        drawLine.ClearLine();
        lr.enabled = true;
        StartCoroutine(EnableDrawingNextFrame());
    }

    public void QuitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else
	 	Application.Quit();
#endif
    }
}