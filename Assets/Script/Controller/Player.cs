using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public bool isGameOver = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bee"))
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
        }
    }
}
