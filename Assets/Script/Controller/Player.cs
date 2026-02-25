using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Sprite egg, brokenEgg, chicEgg;
    public bool isGameOver = false;

    private SpriteRenderer sr;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
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
            || collision.gameObject.CompareTag("Pabble")
            )
        {
            isGameOver = true;
            UIManager.Instance.GameOver();
            Debug.Log("Game Over");

            ChangeImage();

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
}
