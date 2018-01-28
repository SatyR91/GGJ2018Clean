using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SKILLS {
    SWIM
};


public class Unit : MonoBehaviour {

    public UnitState mState;
	public STATE State;
    public List<SKILLS> mSkills;
    public Dictionary<SKILLS, float> mGenes;
    int mTimeInState;
	int mTimeToTeach;
	public int learnTick = 50;
	public Groupe mGroupe;
	public Material LearnMaterial;	

	Animator m_Animator;

	// -------- EVENTS
	// TEACH SKILL
	public delegate void TryTeachingToGroupEvent ();
	public static event TryTeachingToGroupEvent OnTryTeachingToGroupEvent;

    // Chance to learn skills from others
	public float learnChance = 25.0f;
	// Chance to learn each skills
    public float swimChance = 5.0f;

    // Use this for initialization
    void Start () {
		mGroupe = new Groupe();
		mTimeInState = 0;
		mTimeToTeach = 0;
		mState = new MovingState();
		mGenes = new Dictionary<SKILLS, float>();
        randGenes();
		mGroupe = GetComponentInParent<Groupe> ();
		m_Animator = GetComponent<Animator> ();
	}

	void OnEnable() {
		EventManager.OnDrowningEvent += OnDrowningReceived;
		EventManager.OnMovingEvent += OnMovingReceived;
		EventManager.OnDeadEvent += OnDeadReceived;
	}

	void OnDisable() {
		EventManager.OnDrowningEvent -= OnDrowningReceived;
		EventManager.OnMovingEvent -= OnMovingReceived;
		EventManager.OnDeadEvent -= OnDeadReceived;
	}
	
	// Update is called once per frame
	void Update () {
		State = mState.getWeight ();
		float start = Time.time * 1000;
		mState.Execute(mTimeInState, mGenes, m_Animator);
		mTimeInState++;
		mTimeToTeach++;
		TryToTeachSkills();
		float end = Time.time * 1000;
		// Debug.Log ("time spent : " + (end - start));
    }

	// returns true if unit have a skill
	public bool haveSkill(SKILLS skill){
		return mSkills.Contains (skill);
	}

	//Reset current state time
	void resetTimeInState() {
		mTimeInState = 0;
	}

    // Randomize unit chance to learn each skills
    public void randGenes() {
        mGenes.Add(SKILLS.SWIM, UnityEngine.Random.Range(0.0f, swimChance));
    }


	// Event Listeners
	void OnDrowningReceived() {
		if (mSkills.Contains(SKILLS.SWIM))
			return;

		mState = new DrowningState();
		State = STATE.DROWNING;
		// may need to be deleted
		resetTimeInState();
	}

	void OnMovingReceived() {
		if (mState.getWeight () > MovingState.m_weight)
			return;

		mState = new MovingState();
		resetTimeInState();
	}

	void OnDeadReceived() {
		mState = new DeadState ();
		resetTimeInState ();
	}

	// A possible animation here
	void OnSkillLearned() {
		
	}

	void TryToTeachSkills() {
		float start = Time.time*1000;
		//Debug.Log("time start " + start.ToString());
		if (mTimeToTeach > learnTick) {
			Debug.Log("Trying to teach : " + mSkills.Count);
			foreach(SKILLS skill in mSkills) {
				Debug.Log (mGroupe);
				List<Unit> ignorants = mGroupe.GetIgnorants(skill);
				Debug.Log(ignorants.ToString());
				foreach (Unit unit in ignorants) {
					unit.TryToLearnSkill(skill);
				}
			}
			mTimeToTeach = 0;
		}		
		float end = Time.time*1000;
		//Debug.Log("time spent in Try to teach : " + (end - start).ToString());
	}

    public  void TryToLearnSkill(SKILLS aSkill) {
		float chanceToLearn = UnityEngine.Random.Range(0.0f, 100.0f);
		//Debug.Log("Trying to learn : " + chanceToLearn);
        if (chanceToLearn < learnChance) {
			mSkills.Add(aSkill);
			Debug.Log("SKILLLZZZ");
			// TODO : Animation
			GetComponent<Renderer>().material = LearnMaterial;
		}
    }

    public void  invokeTryTeachingToGroup() { OnTryTeachingToGroupEvent(); }
}
