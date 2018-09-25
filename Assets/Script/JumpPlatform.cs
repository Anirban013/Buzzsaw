using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {

	public float jmpMagnitude = 20;

	public void ControllerEnter2D(CharController2D cntrl){
		cntrl.SetVForce(jmpMagnitude);
	}
}
