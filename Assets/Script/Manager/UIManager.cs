using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject startPanel, settingPanel, gameWinPanel, gameOverPanel, gamePlayPanel, pausePanel, quitPanel, nextButton;
    public Image timerImage;
    public TextMeshProUGUI levelText, winText;
    public Sprite[] pauseResumeSprites;
    public GameObject pauseResumeIcon;
    public Sprite[] musicIcons;
    public Sprite[] soundIcons;
    public GameObject music;
    public GameObject sound;

    [SerializeField] private DrawLineWithMouse drawLine;
    private LineRenderer lr;
    private bool isGameRunning = false, isPaused = false, isSettingOpen = false, isMusicOn = true, isSoundOn = true;

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
        settingPanel.SetActive(false);
        gamePlayPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gameWinPanel.SetActive(false);
        pausePanel.SetActive(false);
        quitPanel.SetActive(false);

        lr.enabled = false;
        drawLine.canDraw = false;

        SpawnManager.Instance.DestroyAllSlabs();
    }

    private void Update()
    {
        if (startPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingPanel.activeSelf)
            {
                settingPanel.SetActive(false);
                isSettingOpen = false;
            }
            quitPanel.SetActive(true);
        }
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

        if ((timerImage != null && timerImage.fillAmount <= 0) || SpawnManager.Instance.isAllEnemyDead)
        {
            GameWin();
            if (LevelManager.Instance.currentLevel == 7)
            {
                Player.Instance.WinImage();
            }
        }

        DisplayLevel();
    }

    private void DisplayLevel()
    {
        if (levelText != null)
        {
            levelText.text = "Level " + LevelManager.Instance.currentLevel;
        }

        if (LevelManager.Instance.currentLevel == 17)
        {
            winText.text = "Levels Coming Soon \n Stay Tuned";
            nextButton.SetActive(false);
        }
        else
        {
            winText.text = "Level Complete";
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

        if (LevelManager.Instance.currentLevel != 13)
        {
            SpawnManager.Instance.DestroyAllSlabs();
        }
        else
        {
            SpawnManager.Instance.SpawnSlab();
        }

        SpawnManager.Instance.DestroyAllFans();

        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            SpawnManager.Instance.SpawnFan();
        }
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
    }

    public void GameOver()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!gameWinPanel.activeSelf)
        {
            Handheld.Vibrate();
        }
#endif

        if (timerImage.fillAmount > 0 && !gameWinPanel.activeSelf)
        {
            isGameRunning = false;
            gameOverPanel.SetActive(true);
            gameWinPanel.SetActive(false);
            DrawLineWithMouse.Instance.canDraw = false;
            if (LevelManager.Instance.currentLevel == 13)
                SpawnManager.Instance.DestroyAllSlabs();
        }
    }

    public void BackHome()
    {
        DrawLineWithMouse.Instance.AddKinematic();
        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.DestroyAllPiranha();
        SpawnManager.Instance.DestroyAllKites();
        SpawnManager.Instance.DestroyAllPabbles();
        SpawnManager.Instance.DestroyAllCircles();
        SpawnManager.Instance.DestroyAllSlabs();
        SpawnManager.Instance.DestroyAllFans();
        SpawnManager.Instance.DestroyAllSpiders();
        SpawnManager.Instance.DestroyAllSpiderWebs();
        SpawnManager.Instance.DestroyAllBubbles();
        //SpawnManager.Instance.ResetSpawner();
        SpawnManager.Instance.DestroyPlayer();
        if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);
        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);
        //if (pausePanel.activeSelf)
        //    pausePanel.SetActive(false);
        ClosePausePanel();
        ResetTimer();
        Player.Instance.isGameOver = false;
        SpawnManager.Instance.isAllEnemyDead = false;
        drawLine.ClearLine();
        lr.enabled = false;
        startPanel.SetActive(true);
        LevelManager.Instance.currentLevel = LevelManager.Instance.startLevel;

    }

    public void RetryGame()
    {
        isGameRunning = true;
        DrawLineWithMouse.Instance.hasDrawn = false;
        DrawLineWithMouse.Instance.AddKinematic();
        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.DestroyAllPiranha();
        SpawnManager.Instance.DestroyAllKites();
        SpawnManager.Instance.DestroyAllPabbles();
        SpawnManager.Instance.DestroyAllSpiders();
        SpawnManager.Instance.DestroyAllSpiderWebs();
        SpawnManager.Instance.DestroyAllSlabs();
        SpawnManager.Instance.DestroyAllFans();
        SpawnManager.Instance.DestroyAllBubbles();
        //SpawnManager.Instance.ResetSpawner();

        if (LevelManager.Instance.currentLevel == 13)
        {
            SpawnManager.Instance.SpawnSlab();
        }

        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            SpawnManager.Instance.SpawnFan();
        }

        SpawnManager.Instance.DestroyPlayer();
        SpawnManager.Instance.SpawnPlayer();

        if (LevelManager.Instance.currentLevel == 7 || LevelManager.Instance.currentLevel == 9 || LevelManager.Instance.currentLevel == 10)
        {
            SpawnManager.Instance.DestroyAllCircles();
        }

        if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);

        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);

        //if (pausePanel.activeSelf)
        //    pausePanel.SetActive(false);
        ClosePausePanel();

        ResetTimer();
        Player.Instance.isGameOver = false;
        SpawnManager.Instance.isAllEnemyDead = false;

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

        SpawnManager.Instance.DestroyAllSlabs();
        SpawnManager.Instance.DestroyAllFans();

        if (LevelManager.Instance.currentLevel == 13)
        {
            SpawnManager.Instance.SpawnSlab();
        }
        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            SpawnManager.Instance.SpawnFan();
        }

        SpawnManager.Instance.DestroyPlayer();
        SpawnManager.Instance.SpawnPlayer();

        SpawnManager.Instance.DestroyAllCircles();
        SpawnManager.Instance.DestroyAllBees();
        SpawnManager.Instance.DestroyAllPiranha();
        SpawnManager.Instance.DestroyAllKites();
        SpawnManager.Instance.DestroyAllPabbles();
        SpawnManager.Instance.DestroyAllSpiders();
        SpawnManager.Instance.DestroyAllSpiderWebs();
        SpawnManager.Instance.DestroyAllBubbles();
        //SpawnManager.Instance.ResetSpawner();

        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);

        ResetTimer();
        Player.Instance.isGameOver = false;
        SpawnManager.Instance.isAllEnemyDead = false;
        drawLine.ClearLine();
        lr.enabled = true;
        StartCoroutine(EnableDrawingNextFrame());
    }

    public void ToggleSettingPanel()
    {
        isSettingOpen = !isSettingOpen;

        if (isSettingOpen)
        {
            settingPanel.SetActive(true);
        }
        else
        {
            settingPanel.SetActive(false);
        }
    }

    public void ToggleMusic()
    {
        isMusicOn = !isMusicOn;

        if (isMusicOn)
        {
            music.GetComponent<Image>().sprite = musicIcons[0];
        }
        else
        {
            music.GetComponent<Image>().sprite = musicIcons[1];
        }
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;

        if (isSoundOn)
        {
            sound.GetComponent<Image>().sprite = soundIcons[0];
        }
        else
        {
            sound.GetComponent<Image>().sprite = soundIcons[1];
        }
    }

    public void TogglePauseResume()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            pausePanel.SetActive(true);
            pauseResumeIcon.GetComponent<Image>().sprite = pauseResumeSprites[1];
            Time.timeScale = 0f;
            drawLine.canDraw = false;
        }
        else
        {
            pausePanel.SetActive(false);
            pauseResumeIcon.GetComponent<Image>().sprite = pauseResumeSprites[0];
            Time.timeScale = 1f;
            drawLine.canDraw = true;
        }

        Debug.Log("Paused State: " + isPaused);
    }

    public void ClosePausePanel()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        pauseResumeIcon.GetComponent<Image>().sprite = pauseResumeSprites[0];
        Time.timeScale = 1f;
        drawLine.canDraw = true;
    }

    public void CloseQuitPanel()
    {
        quitPanel.SetActive(false);
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