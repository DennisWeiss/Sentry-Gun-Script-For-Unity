using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour {

    static GameObject thisObj;
    static Vector3 thisVector;
    static float thisSpeed;

    float time;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (thisObj != null)
        {
            thisObj.transform.position += thisVector * thisSpeed * Time.deltaTime;
        }
    }

    public static void Move(GameObject obj, Vector3 vector, float speed) {
        thisObj = obj;
        thisVector = vector;
        thisSpeed = speed;
    }
}
