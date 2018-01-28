using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour {


	public string nextSceneName;
	public Image[] tutoImage;
	public int current = 0;
	public RectTransform tutorialPanel;
	public RectTransform logoPanel;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (tutorialPanel.gameObject.active && Input.GetMouseButton(0)) {
			if (current > tutoImage.Length - 1)
				LoadScene();
			else 
				displayTuto();
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void printNext(){}

	public void displayTuto(){
		logoPanel.gameObject.SetActive(false);
		tutorialPanel.gameObject.SetActive(true);
		if (current > 0)
			tutorialPanel.GetChild(current - 1).gameObject.SetActive(false);
		tutorialPanel.GetChild(current).gameObject.SetActive(true);
		current++;
	}

	public  void QuitGame(){
		Application.Quit();
	}

	public  void LoadScene(){
		SceneManager.LoadScene(nextSceneName);
	}	
}
