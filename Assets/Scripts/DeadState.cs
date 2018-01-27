using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : UnitState {

	public DeadState()
	{
		m_weight = STATE.DEAD;
	}

	~DeadState(){}

	public override void Execute(int time, Dictionary<SKILLS, float> genes)
	{
		EventManager.invokeDie ();
	}
}
