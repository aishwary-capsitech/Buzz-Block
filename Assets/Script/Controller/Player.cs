using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Sprite egg, brokenEgg, chicEgg;
    public bool isGameOver = false;

    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        TogglePlayerBodyType();
        DestroyPlayer();
    }

    private void ChangeImage()
    {
        if (LevelManager.Instance.currentLevel == 7 && brokenEgg != null)
        {
                sr.sprite = brokenEgg;
        }
    }

    public void WinImage()
    {
        if (LevelManager.Instance.currentLevel == 7 && egg != null)
        {
            sr.sprite = chicEgg;
            sr.transform.localRotation = Quaternion.identity;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(isGameOver)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Bee") || collision.gameObject.CompareTag("Kite") || collision.gameObject.CompareTag("Spike") 
            || collision.gameObject.CompareTag("Pabble") || collision.gameObject.CompareTag("Chemical")
            )
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");

            //ChangeImage();

            if (collision.gameObject.CompareTag("Bee"))
            {
                Debug.Log("Collided with Bee");
            }
            else if (collision.gameObject.CompareTag("Kite"))
            {
                Debug.Log("Collided with Kite");
            }
            else if (collision.gameObject.CompareTag("Spike"))
            {
                Debug.Log("Collided with Spike");
            }
            else if (collision.gameObject.CompareTag("Pabble"))
            {
                Debug.Log("Collided with Pabble");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameOver)
        {
            return;
        }

        if ( collision.gameObject.CompareTag("Chemical"))
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");
        }
    }

    private void TogglePlayerBodyType()
    {
        if (LevelManager.Instance.currentLevel == 15 || LevelManager.Instance.currentLevel == 16 || LevelManager.Instance.currentLevel == 17)
        {
            if (DrawLineWithMouse.Instance.hasDrawn)
            {
                rb.gravityScale = 1f;
                rb.bodyType = RigidbodyType2D.Dynamic;
                //Debug.Log(rb.bodyType);
            }
            else
            {
                rb.gravityScale = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
                //Debug.Log(rb.bodyType);
            }
        }
    }

    private void DestroyPlayer()
    {
        float minY = -5.5f, maxY = 5.5f;

        if (gameObject.transform.position.y < minY || gameObject.transform.position.y > maxY)
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");
            //Destroy(gameObject);
        }
    }
}
