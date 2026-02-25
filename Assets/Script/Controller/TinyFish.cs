using UnityEngine;

public class TinyFish : MonoBehaviour
{
    public static TinyFish Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        DestroyTinyFish();
    }

    void DestroyTinyFish()
    {
        float screenHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        float screenHalfHeight = Camera.main.orthographicSize;

        float leftX = -screenHalfWidth;
        float rightX = screenHalfWidth;
        float topY = screenHalfHeight;
        float bottomY = -screenHalfHeight;

        if (gameObject.transform.position.x > rightX + 10f || gameObject.transform.position.x < leftX - 10f || gameObject.transform.position.y > topY + 5f || gameObject.transform.position.y < bottomY - 5f)
        {
            Destroy(gameObject, 0.5f);
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
