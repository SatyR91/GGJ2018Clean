using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

    public float timeToDestroy = 5f;

    private void Start()
    {
        Invoke("KillObject", timeToDestroy);
    }

    private void KillObject()
    {
        Destroy(gameObject);
    }

}
