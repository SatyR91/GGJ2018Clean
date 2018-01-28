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
		Debug.Log (colliderGO.gameObject.name);
		if (colliderGO.gameObject.CompareTag("pikmin")) {
			Debug.Log ("CACA*2");
			Unit collUnit = colliderGO.gameObject.GetComponentInChildren<Unit> ();
				collUnit.transform.position = collUnit.transform.position - 0.5f * new Vector3(0f,1f,0f);
			if (!collUnit.mSkills.Contains(SKILLS.SWIM)) {
				collUnit.State = STATE.DROWNING;
				collUnit.GetComponentInParent<Animator>().SetBool("drowning", true);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("pikmin")) {
			Unit collUnit = other.gameObject.GetComponentInChildren<Unit> ();
			collUnit.State = STATE.MOVING;
			collUnit.GetComponentInParent<Animator>().SetBool("drowning", false);
			collUnit.GetComponentInParent<Animator>().SetBool("swimming", true);
			collUnit.transform.position = collUnit.transform.position + 0.5f * new Vector3(0f,1f,0f);
		}
	}
}
