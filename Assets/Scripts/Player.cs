using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    [SerializeField]
    protected float jumpForce = 3f;

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    protected bool jumping = false;
    protected bool isGrounded = false;
    protected bool inAir = false;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    void FixedUpdate()
    {
        Jump();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.ToLower().Contains("ground"))
        {
            isGrounded = true;
            inAir = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name.ToLower().Contains("ground"))
        {
            isGrounded = false;
            inAir = true;
        }
    }
    public virtual void Inputs()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0) && !inAir)
            {
                jumping = true;
            }
        }
    }

    public virtual void Jump()
    {
        if (jumping && isGrounded)
        {
            //rigidBody.velocity = Vector2.zero;
            rigidBody?.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jumping = false;
        }
    }
}
