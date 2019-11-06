using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class game_controller : MonoBehaviour {

    public GameObject cube;

    // Use this for initialization
	void Start () {
        CreateCircuit();
	}
	
	// Update is called once per frame
	void Update () {
	}

    void CreateCircuit()
    {
        //var theta = 0; -> angle that will be increased each loop
        double h = 0;     // x coordinate of circle center
        double k = 0;     // y coordinate of circle center
        double step = 2 * Math.PI / 20; // amount to add to theta each time (degrees)
        double r = 50;      // radio

        for (double theta = 0; theta < 2 * Math.PI; theta += step)
        {
            double x = h + r * Math.Cos(theta);
            double z = k - r * Math.Sin(theta);    //note 2.
            Instantiate(cube,new Vector3((float)x, 0, (float)z), new Quaternion());
        }
    }
}
