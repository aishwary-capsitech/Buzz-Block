using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject[] levels;
    public int startLevel = 1;
    public int currentLevel = 1;

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
        DisplayLevel();
        SpawnManager.Instance.SpawnPlayer();
    }

    void Update()
    {
        
    }

    public void IncreaseLevel()
    {
        currentLevel++;
        Debug.Log("Level Up! Current Level: " + currentLevel);
    }

    public void DisplayLevel()
    {
        foreach (GameObject level in levels)
        {
            if (level.name.Contains(currentLevel.ToString()))
            {
                level.SetActive(true);
            }
            else
            {
                level.SetActive(false);
            }
        }
    }
}
