using System;
using System.Collections.Generic;
using UnityEngine;

public class DrowningState : UnitState
{
	private int m_DrowningTime;
    public DrowningState()
    {
		m_weight = STATE.DROWNING;
		m_DrowningTime = 500; //equilibrage a faire
    }

    ~DrowningState(){}

	public override void Execute(int time, Dictionary<SKILLS, float> genes)
    {
		if (time > m_DrowningTime) {
			EventManager.invokeDie();
		} else {
			if (UnityEngine.Random.Range (0.0f, 100.0f) < genes[SKILLS.SWIM])
				EventManager.invokeMoving ();
		}
    }
}