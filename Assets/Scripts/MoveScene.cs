using UnityEngine;
using System.Collections;

public class MoveScene : MonoBehaviour {

	public float speed;
	public Transform collision;
	private float x;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Transform t = transform;
		if (collision != null) {
			t = collision;
		}

		x = t.position.x;
		x += speed * Time.deltaTime;

		t.position = new Vector3 (x, t.position.y, t.position.z);

	}
}
