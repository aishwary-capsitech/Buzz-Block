using UnityEngine;

public class Bubble : MonoBehaviour
{
    public static Bubble Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        DestroyBubble();
    }

    void DestroyBubble()
    {
        if (LevelManager.Instance.currentLevel == 0)
        {
            Destroy(gameObject, 1.5f);
        }
        if (LevelManager.Instance.currentLevel == 8) {

            float topY = -3f;

            if(gameObject.transform.position.y > topY)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DestroyOnLevelChange()
    {
        if (LevelManager.Instance.currentLevel == 1)
        {
            Destroy(gameObject);
        }
    }
}
