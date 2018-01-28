using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabDisplay : MonoBehaviour {

	public Image imageToLoad;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Tab)){
			imageToLoad.enabled = true;
		}

		if(Input.GetKeyUp(KeyCode.Tab)){
			imageToLoad.enabled = false;
		}
	}
}
