using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floak : MonoBehaviour {

	public Groupe groupe = null;
	private Vector3 f_MooveVector = new Vector3(1,0,0);
	private Transform f_Child;
	private float f_angle = 0;
	private Unit unit;
	public STATE state;

	public int chanceToTurnBack = 100;

    public Renderer rend;

	void Start () {
		f_Child = transform.GetChild(0);
		unit =  GetComponentInChildren<Unit>();
		state = unit.State;
	}
	
	void MoovingPolicy(){
		if (Random.Range(0,groupe.g_RulesFrequency) < 1){
			ApplyRules();
		}
	}
	
	void DrowningPolicy(){
		// if (Random.Range(0,groupe.g_RulesFrequency) < 1){
			Debug.Log("DRAWNING");
			f_MooveVector = Vector3.zero;
			// ApplyRules();
		// }
	}

	void Update () {
		state = unit.State;
		if (groupe != null && groupe.AllAgents.Length != 0){
		if (state == STATE.MOVING) 
			MoovingPolicy();
		if (state == STATE.DROWNING)
			DrowningPolicy();
		f_Child.rotation = Quaternion.LookRotation(f_MooveVector);
		this.transform.Translate(f_MooveVector*Time.deltaTime*groupe.g_Speed);
		ComputeRepulsion();
		}
	}

	void ApplyRules(){
		var alignment = ComputeAlignement();
		var cohesion = ComputeCohesion();
		var separation = ComputeSeparation();	
		var goalDir = groupe.g_ReachGoal ? ComputeGoal() : new Vector3(0,0,0);	

		f_MooveVector.z += goalDir.z*groupe.g_GoalWeight + alignment.z*groupe.g_AlignementWeight + cohesion.z*groupe.g_CohesionWeight + separation.z*groupe.g_SeparationWeight;
		f_MooveVector.x += goalDir.x*groupe.g_GoalWeight + alignment.x*groupe.g_AlignementWeight + cohesion.x*groupe.g_CohesionWeight + separation.x*groupe.g_SeparationWeight;

		f_MooveVector.Normalize();
	}
	float distanceFrom(Vector3 pos){
		return Mathf.Sqrt(Mathf.Pow(transform.position.x - pos.x,2) + Mathf.Pow(transform.position.z - pos.z,2));
	}

	Vector3 ComputeAlignement(){
		var v = new Vector3();
		var neighborCount = 0;
		foreach (GameObject agent in groupe.AllAgents){
            //Debug.Log(agent + ": current agent");
			var floak = agent.GetComponent<Floak>();
			if (agent != this){
				if (distanceFrom(agent.transform.position)<groupe.g_CohesionDistance){
					v.x += floak.f_MooveVector.x;
					v.z += floak.f_MooveVector.z;
					neighborCount++;
				}
			}
		}
		if (neighborCount == 0 )
			return v;
		else{
			v.x /= neighborCount;
			v.z /= neighborCount;
			v.Normalize(); 
			return v;
		}
	}

	Vector3 ComputeCohesion(){
		var v = new Vector3();
		var neighborCount = 0;
		foreach (var agent in groupe.AllAgents){
			if (agent != this){
				var pos = agent.transform.position;
				if (distanceFrom(pos)<groupe.g_CohesionDistance){
					v.x += pos.x;
					v.z += pos.z;
					neighborCount++;
				}
			}
		}
		if (neighborCount == 0 )
			return v;
		else{
			v.x /= neighborCount;
			v.z /= neighborCount;
			v = new Vector3(v.x - transform.position.x,0, v.z - transform.position.z);
			v.Normalize(); 
			return v;
		}		
	}
	Vector3 ComputeSeparation(){
		var v = new Vector3();
		var neighborCount = 0;
		foreach (var agent in groupe.AllAgents){
			if (agent != this){
				var pos = agent.transform.position;
				if (distanceFrom(pos)<groupe.g_SeparationDistance){
					v.x += pos.x - transform.position.x;
					v.z += pos.z - transform.position.z;
					neighborCount++;
				}
			}
		}
		if (neighborCount == 0 )
			return v;
		else{
			v.x *= -1;
			v.z *= -1;
			v.Normalize();
			return v;
		}		
	}

	Vector3 ComputeGoal(){
		var v = new Vector3();
		v.x = groupe.g_GoalPos.x - transform.position.x;
		v.z = groupe.g_GoalPos.z - transform.position.z;
		v.Normalize();
		return v;	
	}

	void ComputeRepulsion(){
		Ray ray = new Ray(transform.position, f_MooveVector);
		RaycastHit hit;
		if (Physics.Raycast(ray,out hit, 2)){
			if (hit.transform.tag == "wall"){
				Debug.DrawRay(transform.position, f_MooveVector*10, Color.yellow);
				f_MooveVector *= -1;
			}
			else if(hit.transform.tag == "water"){
				if (Random.Range(0,chanceToTurnBack) < 1){
				f_MooveVector *= -1;
			}
		}
		else
			Debug.DrawRay(transform.position, f_MooveVector*2, Color.green);
		Debug.DrawRay(transform.position, f_Child.forward*2, Color.red);
	}
	}

}
