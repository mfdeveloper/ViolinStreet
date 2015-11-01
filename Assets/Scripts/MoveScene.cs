using UnityEngine;
using System.Collections;

public class MoveScene : MonoBehaviour {

	public float speed;
	private float x;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		x = transform.position.x;
		x += speed * Time.deltaTime;

		transform.position = new Vector3 (x, transform.position.y, transform.position.z);
	}
}
