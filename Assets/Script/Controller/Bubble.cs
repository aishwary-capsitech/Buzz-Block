using UnityEngine;

public class Bubble : MonoBehaviour
{
    public static Bubble Instance;

    private Rigidbody2D bubbleRb;

    private void Awake()
    {
        Instance = this;

        bubbleRb = GetComponent<Rigidbody2D>();
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
        if (LevelManager.Instance.currentLevel == 8 || LevelManager.Instance.currentLevel == 9 || LevelManager.Instance.currentLevel == 13) {

            float topY = -3.3f;

            if(gameObject.transform.position.y > topY)
            {
                Destroy(gameObject);
            }
        }
    }

    public void DestroyOnLevelChange()
    {
        Destroy(gameObject);
        SpawnManager.Instance.DestroyBubble(bubbleRb);
    }
}
