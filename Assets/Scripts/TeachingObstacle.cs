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
		Debug.Log ("CACA");
		if (colliderGO.gameObject.CompareTag("pikmin")) {
			Debug.Log ("CACA*2");
			Unit collUnit = colliderGO.gameObject.GetComponentInChildren<Unit> ();
			collUnit.State = STATE.DROWNING;
			collUnit.mState = new DrowningState ();
			EventManager.invokeDrowning ();
		}
	}
}
