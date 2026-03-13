//using UnityEngine;

//public class LevelManager : MonoBehaviour
//{
//    public static LevelManager Instance;

//    public GameObject[] levels;
//    public int startLevel = 0;
//    public int currentLevel = 0;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    void Start()
//    {

//    }

//    void Update()
//    {

//    }

//    public void IncreaseLevel()
//    {
//        currentLevel++;
//        Debug.Log("Level Up! Current Level: " + currentLevel);
//    }

//    public void DisplayLevel()
//    {
//        string targetLevel = "Level" + currentLevel;

//        foreach (GameObject level in levels)
//        {
//            if (level.name == targetLevel)
//            {
//                level.SetActive(true);
//            }
//            else
//            {
//                level.SetActive(false);
//            }
//        }
//    }
//}


using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public GameObject[] levels;
    public int startLevel = 0;
    public int currentLevel = 0;

    private GameObject currentLevelObject;

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
        currentLevel = startLevel;
        DisplayLevel();
    }

    public void IncreaseLevel()
    {
        currentLevel++;

        if (currentLevel >= levels.Length)
        {
            currentLevel = levels.Length - 1;
        }

        Debug.Log("Level Up! Current Level: " + currentLevel);

        DisplayLevel();
    }

    public void DisplayLevel()
    {
        if (currentLevelObject != null)
        {
            Destroy(currentLevelObject);
        }

        currentLevelObject = Instantiate(levels[currentLevel], Vector3.zero, Quaternion.identity);
    }
}