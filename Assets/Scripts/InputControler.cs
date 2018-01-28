using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControler : MonoBehaviour {

    // à déterminer
    public float minDistanceToCut = 0.1f;

    [Header("binding params")]
    public LayerMask horizontalPlaneLayerMask;
    public LayerMask groupLayerMask;
    public string objectTag = "pikmin";


    public List<GameObject> rightObjects;
    public List<GameObject> leftObjects;

    [Range(0f,1f)]
    public float timeForDoubleClick = 0.25f;

    public GameObject trailMakerPrefab;

    private GameObject testPrefab;
    
    private float _lastClickTime;
    private bool _doubleClicking;


    GameObject[] objectsToCheck;
    bool isTracingLine = false;

    [SerializeField] Vector3 _firstLinePoint;
    [SerializeField] Vector3 _secondLinePoint;

    Vector3 _firstPointOnScreen;
    Vector3 _secondPointOnScreen;

    float _rayMaxDistance = 3000f;


    private void Start()
    {
        
        objectsToCheck = GameObject.FindGameObjectsWithTag(objectTag);

        if(trailMakerPrefab == null)
            Debug.LogError("trail maker prefab missing in input controler");
    }

    private void Update()
    {
        _doubleClicking = false;

        // push the button
        if (Input.GetButtonDown("Fire1"))
        {
            _firstPointOnScreen = Input.mousePosition;
            _firstLinePoint = GetMousePositionOnPlane();
            //Instantiate(testPrefab, pos, Quaternion.identity);


            //// check if it is a double click
            //if(Time.time - _lastClickTime < timeForDoubleClick)
            //{
            //    _doubleClicking = true;
            //    // if double click we select/unselect group

            //    GameObject touchedObj = GetObjTouched();
            //    if (touchedObj != null && touchedObj.tag == "group" && !touchedObj.GetComponent<Groupe>().isSelected) // the touched object is a non-selected group
            //    {
            //        // get its controler
            //        Groupe controler = touchedObj.GetComponent<Groupe>();
            //        Groupe.Unselect(); // unselect already selected group
            //        controler.Select(); // select new one
            //    }
            //    else // touched object is not a group or is already selected
            //    {
            //        Groupe.Unselect(); // unselect already selected group
            //    }
            //}

            // update last click time
            _lastClickTime = Time.time;     
            
            // we try to trace a line
            isTracingLine = true;

        }

        // release the left button
        if (Input.GetButtonUp("Fire1") )
        {
            // if we are tracing a line (it should be... )
            if (isTracingLine)
            {
                _secondPointOnScreen = Input.mousePosition;
                _secondLinePoint = GetMousePositionOnPlane();
                

                isTracingLine = false;

                // check if minimum distance for slicing is reached -> slice
                if(CheckMinDistance()) // if we are slicing
                {
                    Debug.Log("over min distance, splitting objects");

                    Groupe slicedGroup = GetSlicedGroup();
                    if(slicedGroup != null)
                    {
                        Debug.Log("group found");

                        //ConstructLeftRightLists();
                        slicedGroup.SeparateGroup(_firstLinePoint, _secondLinePoint);

                    }
                    else
                    {
                        Debug.Log("no group found");

                    }
                    StartCoroutine(CreateTrail(_firstLinePoint,_secondLinePoint));


                }
                else // if slicing distance is not reach -> simple click
                {
                    // try to move selected group
                    GameObject touchedObj = GetObjTouched();
                    if (touchedObj != null) {
                        Debug.Log("Objet non null");
                        Debug.Log("touched object: "+ touchedObj.name);
                        if (touchedObj.tag == "group") {
                            Debug.Log("Objet has tag group");

                            if (!touchedObj.GetComponent<Groupe>().isSelected) {
                                                        Debug.Log("Objet is not selected");

// get its controler
                        Groupe controler = touchedObj.GetComponent<Groupe>();
                        Groupe.Unselect(); // unselect already selected group
                        controler.Select(); // select new one
                            }
                        }
                    }   // the touched object is a non-selected group
                    else // touched object is not a group or is already selected
                    {
                        Groupe.Unselect(); // unselect already selected group
                    }
                }

            }
        }

        // if pressing the right button
        if (Input.GetButtonUp("Fire2"))
        {

            GameObject objectUnderMouse = GetObjTouched();
            if(objectUnderMouse != null && objectUnderMouse.tag == "group" && Groupe.selectedObject != null){
                // we hit a group, we try to merge the two groups

                Groupe clickedGroup = objectUnderMouse.GetComponent<Groupe>();
                if(clickedGroup == null)
                    Debug.LogError("object with tag group has no groupe comp");

                clickedGroup.MergeGroup(Groupe.selectedObject);

            }
            else{ // if no group under mouse

                Debug.Log("try to displace selected group");
                Vector3 newPos = GetMousePositionOnPlane();
                if (Groupe.selectedObject != null)
                {
                    Debug.Log("move selected group to " + newPos);
                    Groupe.ChangeSelectedPosition(newPos);
                }
                else
                {
                    Debug.Log("no group selected, no movement");
                }
            }
        }


        // TO SUPRESS
        if(Input.GetKeyDown(KeyCode.A)){
            Vector3 newPos = Input.mousePosition;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = newPos;
        }

    }

    //// return true if point is left of the line
    //public bool GetPointSide(Vector3 point)
    //{
    //    float d = (_secondLinePoint.x - _firstLinePoint.x) * (point.z - _firstLinePoint.z) - (_secondLinePoint.z - _firstLinePoint.z) * (point.x - _firstLinePoint.x);

    //    return (d > 0);
    //}

    // return true if point is left of the line
    public static bool GetPointSide(Vector3 a, Vector3 b, Vector3 c)
    {
        float d = (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x);

        return (d > 0);
    }

    private void ConstructLeftRightLists()
    {
        int objCount = objectsToCheck.Length;
        rightObjects = new List<GameObject>();
        leftObjects = new List<GameObject>();


        foreach (GameObject obj in objectsToCheck)
        {
            bool side = GetPointSide(_firstLinePoint, _secondLinePoint, obj.transform.position);
            Debug.Log(obj + ": added to the " + (side ? "left" : "rigth") + " side");
            if (side)
            {
                leftObjects.Add(obj);
            }
            else
            {
                rightObjects.Add(obj);
            }
        }
    }

    // check minimum slice distance
    private bool CheckMinDistance()
    {
        float xDist = (_secondPointOnScreen.x - _firstPointOnScreen.x) / Screen.width;
        float yDist = (_secondPointOnScreen.y - _firstPointOnScreen.y) / Screen.height;
        float length = new Vector3(xDist, yDist, 0f).magnitude;
        Debug.Log("cut vector length: "+length);
        return (length > minDistanceToCut);
    }



    private Vector3 GetMousePositionOnPlane()
    {
        Vector3 touchedPos = new Vector3() ;

        // cast ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // if ray touched
        if (Physics.Raycast(ray, out hit, _rayMaxDistance, horizontalPlaneLayerMask.value))
        {
            touchedPos = hit.point;
            //Debug.Log(gameObject + ": casting ray; ray successfully hit at "+touchedPos);
        }
        else
            Debug.LogError(gameObject + ": ray cast : no plane found");

        return touchedPos;
    }

    // return touched object
    private GameObject GetObjTouched()
    {
        GameObject hitGO = null;

        // cast ray
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        // if ray touched
        if (Physics.Raycast(ray, out hit, _rayMaxDistance, groupLayerMask.value)) // add layer
        {
            hitGO = hit.collider.gameObject;
            Debug.Log(gameObject + ": check for group; ray successfully hit");

            // var newGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            // newGo.transform.position = hit.point;
        }
        else
            Debug.Log(gameObject + ": check for group; ray hit nothing");

        return hitGO;
    }


    private Groupe GetSlicedGroup()
    {
        // first we instanciate 2 objects to cast a ray
        GameObject obj1 = new GameObject("obj1");
        obj1.transform.position = _firstLinePoint;
        obj1.AddComponent<DestroyAfterTime>();

        GameObject obj2 = new GameObject("obj2");
        obj2.transform.position = _secondLinePoint;
        obj2.AddComponent<DestroyAfterTime>();

        // DO WE CHECK A MINIMUM SLICE RAY DEPTH ?

        // cast a ray between the 2 objects
        RaycastHit hit;
        if (Physics.Raycast(_firstLinePoint, (_secondLinePoint - _firstLinePoint), out hit, (_secondLinePoint - _firstLinePoint).magnitude, groupLayerMask))
        {
            Debug.Log("sliced hit group");
            return hit.collider.gameObject.GetComponent<Groupe>();
        }

        return null;
    }


    private IEnumerator CreateTrail(Vector3 startPos, Vector3 endPos){
        float trailTime = 15f;
        float startTime = 0f;

        // float trailSpeed = 40f;

        GameObject trailMakerObj = Instantiate(trailMakerPrefab,startPos,Quaternion.identity);
        Vector3 direction = (endPos - startPos);
        direction.Normalize();

        while((trailMakerObj.transform.position - endPos).magnitude > 0.3f){
            startTime += Time.deltaTime;
            trailMakerObj.transform.position = Vector3.Lerp(startPos,endPos,startTime/trailTime);

            
            // trailMakerObj.transform.position = trailMakerObj.transform.position + trailSpeed * Time.deltaTime * direction;

            yield return null;
        }
    }
}
