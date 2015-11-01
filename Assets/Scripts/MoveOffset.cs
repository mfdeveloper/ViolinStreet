using UnityEngine;
using System.Collections;

public class MoveOffset : MonoBehaviour {

	private Material currentMaterial;
	public float speed;
	public float offset;

	// Use this for initialization
	void Start () {
		currentMaterial = GetComponent<Renderer> ().material;
	
	}
	
	// Update is called once per frame
	void Update () {
		offset += speed * Time.deltaTime;

		currentMaterial.SetTextureOffset ("_MainTex", new Vector2 (offset,0));
	}
}
