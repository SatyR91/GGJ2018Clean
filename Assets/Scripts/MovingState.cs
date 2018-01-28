using System;
using System.Collections.Generic;
using UnityEngine;

public class MovingState : UnitState
{
    public MovingState()
    {
		m_weight = STATE.MOVING;
    }

    ~MovingState(){}

	public override void Execute(int time, Dictionary<SKILLS, float> genes, Animator anim)
    {

    }
}