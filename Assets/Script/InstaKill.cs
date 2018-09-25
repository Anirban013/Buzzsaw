using UnityEngine;
using System.Collections;

public class InstaKill : MonoBehaviour {

	// Use this for initialization
	public void OnTriggerEnter2D(Collider2D coll)
    {
        var player = coll.GetComponent<Player>();
        if (player == null)
            return;
        LevelManager.Instance.KillPlayer();
    }
}
