using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Rigidbody2D playerRigid;
	public Animator playerAnimator;
	public int forceJump;

	//Verify if character touch the ground
	public bool grounded;
	public Transform groundCheck;
	public LayerMask whatIsGround;

	private bool enableJump;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		grounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, whatIsGround);

		playerAnimator.SetBool("jump", !grounded);

	}

	void OnTriggerEnter2D(){

		enableJump = true;

	}

	public void JumpTouch() {
		Jump ();
	}

	private void Jump() {

		if (enableJump && grounded) {
			playerRigid.AddForce (new Vector2 (50, forceJump));
			enableJump = false;
		}

	}
}
