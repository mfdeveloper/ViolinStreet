using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    [SerializeField]
    protected float jumpForce = 3f;
    [SerializeField]
    protected GameObject attackShoot;
    [SerializeField]
    protected float attackSpeed = 2f;
    [SerializeField]
    protected bool attackOnAir = true;

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    protected bool jumping = false;
    protected bool isGrounded = false;
    protected bool inAir = false;
    protected List<GameObject> shoots = new List<GameObject>();

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(attackShoot != null)
        {
            if(attackShoot.GetComponent<SpriteRenderer>() == null)
            {
                Debug.LogError("A Sprite component is required for attack Prefab");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    void FixedUpdate()
    {
        Jump();

        Attack();
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
            if (InputManager.touch.IsScreenRight() && !inAir)
            {
                jumping = true;
            }

            if (InputManager.touch.IsScreendLeft() && attackShoot != null)
            {

                if (!attackOnAir)
                {
                    if (!inAir)
                    {
                        shoots.Add(Instantiate(attackShoot, transform));
                    }
                } else
                {
                    shoots.Add(Instantiate(attackShoot, transform));
                }
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

    public virtual void Attack()
    {
        if (shoots.Count > 0)
        {
            for (int i = 0; i < shoots.Count; i++)
            {
                var shoot = shoots[i];
                if (shoot != null)
                {
                    shoot.transform.Translate(Vector2.right * attackSpeed * Time.deltaTime);

                    var shootPos = Camera.main.WorldToViewportPoint(shoot.transform.position);
                    if (shootPos.x >= 1.0f)
                    {
                        Debug.Log("Destroy shoot");

                        shoots.Remove(shoot);
                        Destroy(shoot);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Callback subscribed by event "OnBeat" of <see cref="MetronomePro.events"/> attribute
    /// <b>PS:</b> This is ony for player sync tests. Remove this in future
    /// </summary>
    /// <param name="metronome"></param>
    /// <param name="player"></param>
    public virtual void ChangeColor(MetronomePro metronome, MetronomePro_Player player)
    {
        
        if (transform.childCount > 0)
        {
            var child = transform.GetChild(0);
            child.GetComponentInChildren<SpriteRenderer>().color = Random.ColorHSV(0f, 1f);
        } else if (spriteRenderer != null)
        {
            spriteRenderer.color = Random.ColorHSV(0f, 1f);
        }
    }
}
