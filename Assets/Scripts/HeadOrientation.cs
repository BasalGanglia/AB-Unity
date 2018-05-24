using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Affdex;
using UnityEditor;
using System.IO;
using System;

public class HeadOrientation : MonoBehaviour
{

    private static GameObject head;

    void Start()
    {
  //      head = GameObject.Find("Head_jnt");
		head = GameObject.Find ("AB_Blendshapes");
    }

    void Update()
    {
        //Debug.Log("X:" + (int)(FaceControl.OrientationX * -100) + "Y:" + (int)(FaceControl.OrientationY * -100));  head.transform.rotation.z
        // Rotating the head based on the orientation values we get from Affectiva SDK.

		head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation,Quaternion.Euler((int)(FaceControl.OrientationX* -100)-90, (int)(FaceControl.OrientationY*-100), (int)(FaceControl.OrientationZ * -100)),15*Time.deltaTime);
//		head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation,Quaternion.Euler(-90, (int)(FaceControl.OrientationY*-100), (int)(FaceControl.OrientationZ * -100)),15*Time.deltaTime);

	}

}