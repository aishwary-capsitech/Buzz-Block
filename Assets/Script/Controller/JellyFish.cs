using UnityEngine;

public class JellyFish : MonoBehaviour
{
    public static JellyFish Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        DestroyJelly();
    }

    void DestroyJelly()
    {
        float screenHalfHeight = Camera.main.orthographicSize;
        float topY = screenHalfHeight;

        if (gameObject.transform.position.y > topY)
        {
            Destroy(gameObject, 0.7f);
        }
    }

    public void DestroyOnLevelChange()
    {
        if (LevelManager.Instance.currentLevel != 0)
        {
            return;
        }
        Destroy(gameObject);
    }
}
