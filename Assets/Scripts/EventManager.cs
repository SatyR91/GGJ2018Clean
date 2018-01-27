using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

	// MOVING
	public delegate void MovingEvent ();
	public static event MovingEvent OnMovingEvent;

	// DROWNING
	public delegate void DrowningEvent ();
	public static event DrowningEvent OnDrowningEvent;

	// DEAD
	public delegate void DeadEvent ();
	public static event DeadEvent OnDeadEvent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	static public void invokeDrowning() {	OnDrowningEvent(); }
	static public void invokeMoving() { OnMovingEvent(); }
	static public void invokeDie() { OnDeadEvent ();}

}


