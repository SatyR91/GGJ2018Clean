using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachingObstacle : MonoBehaviour {

	private STATE m_State;

	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider colliderGO) {
		if (colliderGO.gameObject.CompareTag("pikmin")) {
			Unit collUnit = colliderGO.gameObject.GetComponentInChildren<Unit> ();
			EventManager.invokeDrowning ();
		}
	}
}
