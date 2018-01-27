using System;
using System.Collections.Generic;

public enum STATE {
	MOVING = 1,
	LEARNING = 2,
	DROWNING = 3,
	DEAD = 10
}

public abstract class UnitState {
    public static STATE m_weight;
	public STATE getWeight() {
		return m_weight;
	}

	public abstract void Execute(int time, Dictionary<SKILLS, float> genes);
}
