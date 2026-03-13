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
    public Sprite[] vibrateIcons;
    public GameObject music;
    public GameObject sound;
    public GameObject vibrate;
    public GameObject[] musicObjects;
    public GameObject[] soundObjects;
    public GameObject[] vibrateObjects;

    [SerializeField] private DrawLineWithMouse drawLine;
    private LineRenderer lr;
    private bool isGameRunning = false, isPaused = false, isSettingOpen = false, isMusicOn = true, isSoundOn = true, isVibrateOn = true;

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
        AudioManager.Instance.BgMusic();
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
        if (!isGameRunning || Player.Instance == null)
        {
            return;
        }

        if (drawLine.hasDrawn && !Player.Instance.isGameOver && !gameWinPanel.activeSelf)
        {
            UpdateTimer();
        }

        if ((((timerImage != null && timerImage.fillAmount <= 0) || SpawnManager.Instance.isAllEnemyDead) && LevelManager.Instance.currentLevel != 21))
        {
            GameWin();
            if (LevelManager.Instance.currentLevel == 7)
            {
                Player.Instance.WinImage();
            }
        }

        if ((LevelManager.Instance.currentLevel == 21 && Player.Instance.isReachedFinish))
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

        if (LevelManager.Instance.currentLevel == 21)
        {
            winText.text = "Levels Coming Soon \n Stay Tuned";
            nextButton.SetActive(false);
            //nextButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            winText.text = "Level Complete";
        }
    }

    public void StartGame()
    {
        AudioManager.Instance.Tap();

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
        drawLine.canDraw = false;
    }

    public void GameOver()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (!gameWinPanel.activeSelf && isVibrateOn)
        {
            Handheld.Vibrate();
        }
#endif

        if (timerImage.fillAmount > 0 && !gameWinPanel.activeSelf)
        {
            isGameRunning = false;
            gameOverPanel.SetActive(true);
            gameWinPanel.SetActive(false);
            drawLine.canDraw = false;
            //if (LevelManager.Instance.currentLevel == 13)
            //    SpawnManager.Instance.DestroyAllSlabs();
        }
    }

    public void BackHome()
    {
        AudioManager.Instance.Tap();

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
        if (Player.Instance != null)
        {
            Player.Instance.isGameOver = false;
            Player.Instance.isReachedFinish = false;
            Debug.Log("Finish : " + Player.Instance.isReachedFinish);
        }
        SpawnManager.Instance.isAllEnemyDead = false;
        drawLine.ClearLine();
        drawLine.isStartedDrawing = false;
        Debug.Log(drawLine.isStartedDrawing);
        lr.enabled = false;
        startPanel.SetActive(true);
        LevelManager.Instance.currentLevel = LevelManager.Instance.startLevel;
        SpawnManager.Instance.beeCount = 0;
    }

    public void RetryGame()
    {
        AudioManager.Instance.Tap();

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

        if (LevelManager.Instance.currentLevel == 7 || LevelManager.Instance.currentLevel == 9 || LevelManager.Instance.currentLevel == 10 || LevelManager.Instance.currentLevel == 18)
        {
            SpawnManager.Instance.DestroyAllCircles();
        }

        //if (LevelManager.Instance.currentLevel == 20)
        //{
        //    Pendulum.Instance.ResetPendulum();
        //}

        if (gameOverPanel.activeSelf)
            gameOverPanel.SetActive(false);

        if (gameWinPanel.activeSelf)
            gameWinPanel.SetActive(false);

        //if (pausePanel.activeSelf)
        //    pausePanel.SetActive(false);
        ClosePausePanel();

        ResetTimer();
        Player.Instance.isGameOver = false;
        Player.Instance.isReachedFinish = false;
        Debug.Log("Finish : " + Player.Instance.isReachedFinish);
        SpawnManager.Instance.isAllEnemyDead = false;

        drawLine.ClearLine();
        lr.enabled = true;

        drawLine.isStartedDrawing = false;

        StartCoroutine(EnableDrawingNextFrame());

        SpawnManager.Instance.beeCount = 0;
    }

    public void NextGame()
    {
        AudioManager.Instance.Tap();

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
        AudioManager.Instance.Tap();

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
        AudioManager.Instance.BgMusicControl(isMusicOn);
        AudioManager.Instance.Tap();

        if (isMusicOn)
        {
            foreach (GameObject music in musicObjects)
                music.GetComponent<Image>().sprite = musicIcons[0];
        }
        else
        {
            foreach (GameObject music in musicObjects)
                music.GetComponent<Image>().sprite = musicIcons[1];
        }
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        AudioManager.Instance.SFXControl(isSoundOn);
        AudioManager.Instance.Tap();

        if (isSoundOn)
        {
            foreach (GameObject sound in soundObjects)
                sound.GetComponent<Image>().sprite = soundIcons[0];
        }
        else
        {
            foreach (GameObject sound in soundObjects)
                sound.GetComponent<Image>().sprite = soundIcons[1];
        }
    }

    public void ToggleVibration()
    {
        isVibrateOn = !isVibrateOn;
        AudioManager.Instance.Tap();

        if (isVibrateOn)
        {
            foreach (GameObject vibrate in vibrateObjects)
                vibrate.GetComponent<Image>().sprite = vibrateIcons[0];
        }
        else
        {
            foreach (GameObject vibrate in vibrateObjects)
                vibrate.GetComponent<Image>().sprite = vibrateIcons[1];
        }
    }

    public void TogglePauseResume()
    {
        isPaused = !isPaused;
        AudioManager.Instance.Tap();

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
        AudioManager.Instance.Tap();
        isPaused = false;
        pausePanel.SetActive(false);
        pauseResumeIcon.GetComponent<Image>().sprite = pauseResumeSprites[0];
        Time.timeScale = 1f;
        drawLine.canDraw = true;
    }

    public void CloseQuitPanel()
    {
        AudioManager.Instance.Tap();
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