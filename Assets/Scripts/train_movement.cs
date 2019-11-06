using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class train_movement : MonoBehaviour {

    private Vector3 pos_original;

	// Use this for initialization
	void Start () {
        pos_original = gameObject.transform.position;

    }
	
	// Update is called once per frame
	void Update () {
		if(gameObject.transform.position.z < 280)
        {
            //avanzo el tren
            gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, Time.deltaTime * 21);
        }
        else
        {
            //lo restauro a la posicion original
            gameObject.transform.position = pos_original;
        }
	}
}
