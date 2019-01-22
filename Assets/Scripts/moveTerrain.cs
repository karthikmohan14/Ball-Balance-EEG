using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveTerrain : MonoBehaviour
{

    float smooth = 5.0f;
    float tiltAngle = 60.0f;
    float x, z;
    int frames;

    //Vector2 yay = MindwaveCalibrator.m_DeltaWaves;
    // Brainwave _BrainwaveType;
    // Use this for initialization
    void Start()
    {
        frames = 0;
        x = Random.Range(1, 5);
        z = Random.Range(1, 5);

    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(yay.x);
        frames++;
        // Smoothly tilts a transform towards a target rotation.
        float tiltAroundZ = Input.GetAxis("Horizontal") * -tiltAngle;
        float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;

        Quaternion target;
        if (frames > 50)
        {
            target = Quaternion.Euler(tiltAroundX + x, 0, tiltAroundZ + z);
            x *= 1.1f;
            z *= 1.1f;
            frames = 0;
        }
        else
        {
            target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
        }
        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);

    }
}