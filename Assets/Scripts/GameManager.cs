using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gm;
    public GameObject groupePrefab;

    private void Awake()
    {
        CheckSingleton();
    }

    void Start(){
        GameObject groupeObj = Instantiate(groupePrefab,Vector3.zero,Quaternion.identity);
        Groupe groupe = groupeObj.GetComponent<Groupe>();
        groupe.AllAgents = new GameObject[groupe.g_NumFloakAgents];
        groupe.SetupScene();
    } 

    private void CheckSingleton()
    {
        if (gm == null)
        {
            gm = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
