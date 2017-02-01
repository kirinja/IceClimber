using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is meant to switch between FirstPersonCamera and ThirdPersonOrbit depending on input
/// In many platformers you have the option to look around to get better understanding of your surroundings
/// ThirdPersonOrbit doesnt allow for much of that, so preferably we would want to switch to a first person view at a button press
/// and disable the third person camera. Then when we're done we switch back
/// 
/// In the future we might also want some kind of EventCamera and this should manage that as well
/// We could probably bypass this entire script by having several cameras and activate/deactivate them as we see fit?
/// </summary>
public class CameraManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
