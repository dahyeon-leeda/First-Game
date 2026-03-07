using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    GameManager gameManager;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Finish"))
        {
            gameManager.MoveToLevel1Random();
        }
        else if (collision.CompareTag("shop"))
        {
            Debug.Log("shop에 닿음");
            gameManager.PlayerPositionShop();
        }
    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
}