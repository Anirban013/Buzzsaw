using UnityEngine;
using System.Collections;
using System;

public class PathedProjectile : MonoBehaviour, ITakeDamage {

    // Use this for initialization
    public int pointsToGiveToPlayer;

    private Transform _destination;
    private float _speed;

    public GameObject destroyEffect;
	
   public void Initialize(Transform destination, float speed)
    {
        _destination = destination;
        _speed = speed;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, _destination.position, Time.deltaTime * _speed);

        var distanceSquared = (_destination.transform.position - transform.position).sqrMagnitude;

        if (distanceSquared > 0.01f * 0.01f)
            return;

        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if (destroyEffect != null)
            Instantiate(destroyEffect, transform.position, transform.rotation);
        Destroy(gameObject);

        var projectile = instigator.GetComponent<Projectile>();
        if (projectile != null && projectile.Owner.GetComponent<Player>() != null && pointsToGiveToPlayer != 0)
        {
            GameManager.Instance.AddPoints(pointsToGiveToPlayer);
            FloatingText.Show(string.Format("+{0}", pointsToGiveToPlayer), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
        }
    }
}
