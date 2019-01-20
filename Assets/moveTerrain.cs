using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTerrain : MonoBehaviour {

    float smooth = 5.0f;
    float tiltAngle = 60.0f;
    
    int x, z;
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        
        // Smoothly tilts a transform towards a target rotation.
        float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
        x = Random.Range(1, 5);
        z = Random.Range(1, 5);


        Quaternion target = Quaternion.Euler(tiltAroundX+x, 0, tiltAroundZ+z);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * smooth);
	
}
}