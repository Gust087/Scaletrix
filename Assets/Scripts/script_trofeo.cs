using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class script_trofeo : MonoBehaviour {

    public GameObject car_father;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(car_father.transform.position.x, car_father.transform.position.y, car_father.transform.position.z);
        transform.Rotate(new Vector3(0, 1f, 0));
	}
}
