using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform player;
	public Vector2 
		smoothing,
		margin;
	public BoxCollider2D bounds;

	private Vector3 
		_min,
		_max;

	public bool isFollowing { get; set; }

	public void Start(){
		_min = bounds.bounds.min;
		_max = bounds.bounds.max;
		isFollowing = true;
	}

	public void Update(){
		var x = transform.position.x;
		var y = transform.position.y;

		if (isFollowing) {
			if (Mathf.Abs (x - player.position.x) > margin.x)
				x = Mathf.Lerp (x, player.position.x, smoothing.x * Time.deltaTime);
			
			if (Mathf.Abs (y - player.position.y) > margin.y)
				y = Mathf.Lerp (y, player.position.y, smoothing.y * Time.deltaTime);
		}
		var camHalfWidth =  GetComponent<Camera>().orthographicSize * ((float) Screen.width /Screen.height);

		x = Mathf.Clamp (x, _min.x + camHalfWidth, _max.x - camHalfWidth);
		y = Mathf.Clamp (y, _min.y + GetComponent<Camera>().orthographicSize, _max.y - GetComponent<Camera>().orthographicSize);

		transform.position = new Vector3 (x,y, transform.position.z);
	}
}
