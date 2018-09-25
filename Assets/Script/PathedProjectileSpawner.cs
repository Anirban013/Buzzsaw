using UnityEngine;
using System.Collections;
using System;

public class PathedProjectileSpawner : MonoBehaviour{

    // Use this for initialization
    public Transform destination;
    public PathedProjectile projectile;
    public GameObject spawnEffect;
    public float speed;
    public float fireRate;

    private float _nextShotInSeconds;
	void Start () {
        _nextShotInSeconds = fireRate;
	}
	
	// Update is called once per frame
	void Update () {
        if ((_nextShotInSeconds -= Time.deltaTime) > 0)
            return;

        _nextShotInSeconds = fireRate;
        var _projectile = (PathedProjectile)Instantiate(projectile, transform.position, transform.rotation);
        _projectile.Initialize(destination, speed);

        if (spawnEffect != null)
            Instantiate(spawnEffect, transform.position, transform.rotation);
	
	}

    public void OnDrawGizmos()
    {
        if (destination == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, destination.position); 
    }
}
