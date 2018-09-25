using UnityEngine;
using System.Collections;

public class GiveDamageToPlayer : MonoBehaviour {

    // Use this for initialization
    public int damageToGive = 10;

    private Vector2 _lastPosition, _velocity; 
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
        _velocity = (_lastPosition - (Vector2)transform.position) / Time.deltaTime;
	}

    public void OnTriggerEnter2D(Collider2D coll)
    {
        var player = coll.GetComponent<Player>();
        if (player == null)
            return;

        player.TakeDamage(damageToGive, gameObject);
        var controller = player.GetComponent<CharController2D>();

        var totalVelocity = controller.Velocity + _velocity;

        controller.SetForce(new Vector2(
            -1 * Mathf.Sign(totalVelocity.x) * Mathf.Clamp(Mathf.Abs(totalVelocity.x) * 5, 10, 20),
           -1 * Mathf.Sign(totalVelocity.y) * Mathf.Clamp(Mathf.Abs(totalVelocity.y) * 3, 3, 10)
            ));

    }
}
