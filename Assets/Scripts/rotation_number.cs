using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation_number : MonoBehaviour {
    
    public float turnSpeed;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
    }
}
