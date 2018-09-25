using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ControllerParameters2D {
	public enum JmpBehaviour{
		canJmpAnywhere,
		canJmpOnGround,
		cantJmp
	}
	public Vector2 moveVelocity =new Vector2(float.MaxValue, float.MaxValue);
	[Range(0,90)]
	public float slopeLimit = 30;
	public float gravity = -25f;
	public JmpBehaviour jmpRestriction;
	public float jmpFreq = 0.25f;
	public float jmpMagnitude = 12f;
}
