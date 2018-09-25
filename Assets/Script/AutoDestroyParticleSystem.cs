using UnityEngine;
using System.Collections;

public class AutoDestroyParticleSystem : MonoBehaviour {

    // Use this for initialization
    private ParticleSystem _particleSystem;
	void Start () {
        _particleSystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (_particleSystem.isPlaying)
            return;

        Destroy(gameObject);
	}
}
