using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScrolling : MonoBehaviour {

	public float scrollingSpeed = -0.5f;
	Vector3 scrollingVector;
	// Use this for initialization
	void Start () {
		scrollingVector = new Vector3(0,0,scrollingSpeed);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(scrollingVector);
	}
}
