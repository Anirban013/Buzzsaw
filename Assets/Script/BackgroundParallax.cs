using System;
using UnityEngine;


public class BackgroundParallax : MonoBehaviour 
{
	public Transform[] Background;
	public float ParallaxScale;
	public float ParallaxReductionFactor;
	public float Smoothing;

	private Vector2 _lastPosition;

	public void Start()
	{
		_lastPosition = transform.position;
	}

	public void Update(){
		var parallax = (_lastPosition.x - transform.position.x) * ParallaxScale;

		for (var i = 0; i < Background.Length; i++) {
			var backgroundTargetPosition = Background [i].position.x + parallax * (i * ParallaxReductionFactor + 1);
			Background [i].position = Vector3.Lerp (Background [i].position, 
				new Vector3 (backgroundTargetPosition, Background [i].position.y, Background [i].position.z),
				Smoothing * Time.deltaTime
			);

		}
		_lastPosition = transform.position;
	}
}


