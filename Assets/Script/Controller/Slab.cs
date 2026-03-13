using UnityEngine;

public class Slab : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        //DestroySlab();
    }

    void DestroySlab()
    {
        float bottomY = -4.5f;

        if (LevelManager.Instance.currentLevel == 13)
        {
            if (gameObject.transform.position.y < bottomY)
            {
                Destroy(gameObject);
            }
        }
    }
}
