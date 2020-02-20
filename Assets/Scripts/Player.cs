using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    [SerializeField]
    protected float jumpForce = 3f;

    [SerializeField]
    protected GameObject attackShoot;
    [SerializeField]
    protected GameObject specialShoot;

    [SerializeField]
    [Tooltip("One shoot until that leaves of the camera view")]
    protected bool oneShoot = false;
    [SerializeField]
    protected bool attackOnAir = true;
    [SerializeField]
    protected float attackSpeed = 2f;

    [Header("Rhythm Sync")]
    [Space(4)]

    [SerializeField]
    protected MetronomePro metronomePro;

    protected Rigidbody2D rigidBody;
    protected SpriteRenderer spriteRenderer;

    protected bool jumping = false;
    protected bool isGrounded = false;
    protected bool inAir = false;
    protected List<GameObject> shoots = new List<GameObject>();

    protected PlatformRhythm platformRhythm;
    protected Color[] rhythmColors = new[] { Color.yellow, Color.white };

    public PlatformRhythm PlatformJump
    {
        get { return platformRhythm; }
        set { platformRhythm = value; }
    }


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

    void OnEnable()
    {
        if (metronomePro != null)
        {
            metronomePro.events.OnBeat.AddListener(ActionsRhythm);
        }
    }

    void OnDisable()
    {
        if (metronomePro != null)
        {
            metronomePro.events.OnBeat.RemoveListener(ActionsRhythm);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    void FixedUpdate()
    {
        //Jump();

        Attack();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.ToLower().Contains("ground"))
        {
            isGrounded = true;
            inAir = false;

            if (PlatformJump != null)
            {
                PlatformJump = null;
            }
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
        // TODO: Refactor this and move this Inputs to C# events
        // BUG: Tap is executed many times
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            TouchOrMouse.TouchTypeFlags touchFlags = InputManager.touch.TapOrSwipe();

            if (touchFlags.swipe && touchFlags.right)
            {
                //Debug.Log("SPECIAAAAL");
                SpawnShoot("special");
                InputManager.touch.StopSwipe();
            } else
            {
                var touchPosition = InputManager.touch.GetTapPosition(touchFlags);

                if (InputManager.touch.IsScreenRight(touchPosition) && !inAir)
                {
                    jumping = true;
                }

                if (InputManager.touch.IsScreenLeft(touchPosition))
                {
                    SpawnShoot();
                }
            }
        }

    }

    public virtual void SpawnShoot(string type = "normal")
    {
        var shootPrefab = type.Equals("special") ? specialShoot : attackShoot;

        if (shootPrefab == null)
        {
            return;
        }

        if (!attackOnAir)
        {
            if (!inAir)
            {
                if (oneShoot)
                {
                    if (shoots.Count == 0)
                    {
                        shoots.Add(Instantiate(shootPrefab, transform));
                    }

                }
                else
                {
                    shoots.Add(Instantiate(shootPrefab, transform));
                }
            }
        }
        else
        {
            if (shoots.Count == 0)
            {
                shoots.Add(Instantiate(attackShoot, transform));
            }
        }
    }

    public virtual void Jump()
    {
        if (jumping && isGrounded)
        {
            //rigidBody.velocity = Vector2.zero;
            PlatformJump.animatorController?.SetBool("disapear", true);
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

            if (!child.name.Contains("Clone"))
            {
                var childSprite = child.GetComponentInChildren<SpriteRenderer>();

                if (childSprite != null)
                {
                    if (metronome.CurrentStep > 0)
                    {
                        childSprite.color = rhythmColors[metronome.CurrentStep - 1]; // Random.ColorHSV(0f, 1f);
                    }
                }
            }
        } else if (spriteRenderer != null)
        {
            spriteRenderer.color = Random.ColorHSV(0f, 1f);
        }
    }

    public virtual void ActionsRhythm(MetronomePro metronome, MetronomePro_Player player)
    {
        if (metronome.CurrentStep > 0 && metronome.CurrentStep <= player.Base )
        {
            if (PlatformJump != null)
            {
                

                Jump();


                //PlatformJump = null;
            }
        }
    }
}
