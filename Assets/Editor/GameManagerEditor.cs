using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class CameraControlerScript : Editor
{
    GameManager gm;

    private void OnEnable()
    {
        gm = (GameManager)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

		if(GUILayout.Button("Instanciate group"))
		{
            gm.groupePrefab.GetComponent<Groupe>().OnClickInstantiate();

        }
    }
}