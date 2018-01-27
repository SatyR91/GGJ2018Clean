using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Groupe : MonoBehaviour {

    [SerializeField]
    public GameObject[] AllAgents /*{get;set;}*/;
    public GameObject g_FloakingPrefab;
    public GameObject g_GroupPrefab;
    public GameObject g_Camera;
	public Vector3 g_GoalPos = Vector3.zero;
	public Vector3 g_CameraPos = new Vector3(0,1,-25);
    public Transform g_SpawnLocation;


	[Space(15)]
	public int g_NumFloakAgents = 0;
	public bool g_ReachGoal = true;
	[Range(1,100)]
	public float g_Speed = 10f;
	[Range(1,100)]
	public int g_RulesFrequency = 10;
	[Range(0,100)]
	public float g_GroupeRadius = 10;	


	[Space(15)]
	[Range(0,10)]
	public float g_AlignementWeight =1f;
	[Range(0,10)]
	public float g_CohesionWeight = 1f;
	[Range(0,10)]
	public float g_SeparationWeight = 3f;
    [Range(0,10)]
    public float g_GoalWeight = 1f;


	[Space(15)]
	[Range(0,100)]
	public float g_SeparationDistance =2f;	
	[Range(0,100)]
	public float g_CohesionDistance = 15f;

	public bool g_InitScene = false;


    public bool isSelected;
    public float distanceForNewGroup;
    public float minMergeDistance;
    public Material selectedMaterial;
    public  Material defaultMaterial;
    public  Material mouseOverMaterial;
    private InputControler _inputControler;

    public float speed = 1.0f;
    public static Groupe selectedObject = null;


    // public static Groupe selectedObject = null;
	void Start () {
        _inputControler = GameManager.gm.GetComponent<InputControler>();
	}
	
	void Update () {
		if (g_InitScene == true){
			g_InitScene = false;
			g_NumFloakAgents = 10;
			AllAgents = new GameObject[g_NumFloakAgents];
			SetupScene();
		}
        // update center position
        Vector3 barycenter = GetBarycenter(AllAgents);
        this.transform.position = barycenter;
        Vector3 moveDir = new Vector3(-Input.GetAxis("Horizontal"), 0,-Input.GetAxis("Vertical"));
		moveDir = transform.TransformDirection(moveDir);
		moveDir *= speed;
		g_GoalPos+=(moveDir);        

	}

	public void SetupScene(){
		for (var i=0; i<g_NumFloakAgents;i++){
			var groupRadius = Random.insideUnitCircle*g_GroupeRadius;
			var pos = new Vector3(groupRadius.x,0,groupRadius.y) + g_SpawnLocation.position;
			AllAgents[i] = Instantiate(g_FloakingPrefab,pos,Quaternion.identity);
			Floak floak = AllAgents[i].GetComponent<Floak>();
			floak.groupe = this;
            g_GoalPos = g_SpawnLocation.position;

			//AllAgents[i].transform.parent = gameObject.transform;
		}	
	}

	public void SetupGroup(GameObject[] floakObjectList){

        Debug.Log(gameObject + ": setup group; " + floakObjectList.Length + " floaks found");

        // bind each floak
		foreach (GameObject obj in floakObjectList){
			Floak floak = obj.GetComponent<Floak>();
			floak.groupe = this;
		}

        // init the float list
        this.AllAgents = floakObjectList;

        // set goal direction
        g_GoalPos = GetBarycenter(floakObjectList);
	}

    private void OnMouseOver()
    {
        if (!isSelected)
        {
            //Debug.Log("on mouse over !");
            ChangeElementsColor(mouseOverMaterial);
        }
    }

    private void OnMouseExit()
    {
        if (!isSelected)
        {
            ChangeElementsColor(defaultMaterial);
        }
    }


    public void Select()
    {
        Debug.Log(gameObject + ": now selected");
        selectedObject = this;
        isSelected = true;
        ChangeElementsColor(selectedMaterial);
    }

    public static void Unselect()
    {
        Debug.Log("unselect");
        Groupe previousGroup = selectedObject;

        if (selectedObject != null)
        {
            previousGroup.isSelected = false;
            previousGroup.ChangeElementsColor(selectedObject.defaultMaterial);
            selectedObject = null;
        }
        
    }
	
    public static void ChangeSelectedPosition(Vector3 newPos)
    {
        //selectedObject.transform.position = newPos;
        selectedObject.g_GoalPos = newPos;

    }

    private void ChangeElementsColor(Material newMaterial)
    {
        foreach(GameObject GO in AllAgents)
        {
            Floak floak = GO.GetComponent<Floak>();
            if (floak != null)
            {
                //Debug.Log(gameObject + ": change mat");
                floak.rend.material = newMaterial;
            }
            else
            {
                Debug.LogWarning(gameObject + ": no rend found");
            }
        }
        
    }

    // point 1 and 2 defining a line
    public void SeparateGroup(Vector3 point1, Vector3 point2)
    {
        Debug.Log(gameObject + ": separating");
        int objCount = AllAgents.Length;
        List<GameObject>  rightObjects = new List<GameObject>();
        List<GameObject>  leftObjects = new List<GameObject>();

        Vector3 lineNormal = point2 - point1;


        foreach (GameObject floakObj in AllAgents)
        {
            bool side = InputControler.GetPointSide(point1,point2, floakObj.transform.position);
            //Debug.Log(floakObj + ": added to the " + (side ? "left" : "rigth") + " side");
            if (side)
            {
                leftObjects.Add(floakObj);
            }
            else
            {
                rightObjects.Add(floakObj);
            }
        }

        _inputControler.rightObjects = rightObjects;
        _inputControler.leftObjects = leftObjects;

        // if we actually separate something
        if (rightObjects.Count != 0 && leftObjects.Count != 0)
        {
            Unselect();

            Vector3 newRightPos = GetBarycenter(rightObjects) + distanceForNewGroup *  Vector3.Cross(lineNormal,Vector3.up);
            Vector3 newLeftPos = GetBarycenter(leftObjects) + distanceForNewGroup * Vector3.Cross(lineNormal, -Vector3.up);

            GameObject newRightGroup = Instantiate(g_GroupPrefab, Vector3.zero, Quaternion.identity);
            newRightGroup.GetComponent<Groupe>().SetupGroup(rightObjects.ToArray());
            newRightGroup.GetComponent<Groupe>().g_GoalPos = newRightPos;

            GameObject newLeftGroup = Instantiate(g_GroupPrefab, Vector3.zero, Quaternion.identity);
            newLeftGroup.GetComponent<Groupe>().SetupGroup(leftObjects.ToArray());
            newRightGroup.GetComponent<Groupe>().g_GoalPos = newLeftPos;



            Destroy(gameObject);
        }
    }

    public void MergeGroup(Groupe groupToMerge)
    {
        Debug.Log("merging with selected");
        Unselect();
        
        Vector3 newPos = (this.transform.position + groupToMerge.transform.position) / 2;

        GameObject newGroupObj = Instantiate(g_GroupPrefab, Vector3.zero, Quaternion.identity);

        // !!!! used linq !!!!
        GameObject[] newGoVector = this.AllAgents.Concat(groupToMerge.AllAgents).ToArray();

        newGroupObj.GetComponent<Groupe>().SetupGroup(newGoVector);




        // unbind
        Destroy(groupToMerge.gameObject);
        Destroy(gameObject);


    }

    // useless
    private void CheckForMerge() {
        GameObject[] groups = GameObject.FindGameObjectsWithTag("group");
        foreach(GameObject groupObj in groups)
        {
            Groupe group = groupObj.GetComponent<Groupe>();
            
            // if 2 groups are to close
            if((group.transform.position - transform.position).magnitude < minMergeDistance)
            {
                this.MergeGroup(group.GetComponent<Groupe>());
            }

        }
    }

    private static Vector3 GetBarycenter(List<GameObject> GOs)
    {
        Vector3 sum = Vector3.zero;
        foreach (GameObject go in GOs)
        {
            sum += go.transform.position;
        }
        return sum / GOs.Count;
    }

    private static Vector3 GetBarycenter(GameObject[] GOs)
    {
        Vector3 sum = Vector3.zero;

        int counter = 0;

		if(GOs.Length == 0)
			return Vector3.zero;

        foreach (GameObject go in GOs)
        {
            if(go != null)
            {
                sum += go.transform.position;
                counter++;
            }
        }

        if(counter == 0)
        {
            return Vector3.zero;
        }
        else
        {
            return sum / GOs.Length;
        }
    }

	// Returns all Units GameObjects who doesn't have the param skill
	public List<Unit> GetIgnorants(SKILLS skill) {
		List<Unit> resultList = new List<Unit>();
		foreach (GameObject obj in AllAgents) {
			Unit unitComponent = obj.GetComponentInChildren<Unit> ();
			if (unitComponent != null && !unitComponent.haveSkill (skill)) {
				resultList.Add (unitComponent);
			}
		}
		return resultList;
	}

	
}
