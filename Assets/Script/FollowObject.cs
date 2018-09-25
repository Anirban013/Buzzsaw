using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

    // Use this for initialization
    public Vector2 offset;
    public Transform follow;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = follow.transform.position + (Vector3)offset;	
	}
}
