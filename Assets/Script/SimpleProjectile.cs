using UnityEngine;
using System.Collections;
using System;

public class SimpleProjectile : Projectile, ITakeDamage {

    // Use this for initialization
    public int damage;
    public GameObject destroyedEffect;
    public int pointsToGiveToPlayer;
    public float timeToLive;

    public void Update()
    {
        if((timeToLive -= Time.deltaTime) <= 0)
        {
            DestroyProjectile();
            return;
        }
       // transform.Translate((Direction + new Vector2(InitialVelocity.x, 0)) * speed * Time.deltaTime, Space.World);
        transform.Translate(Direction * ((Mathf.Abs(InitialVelocity.x) + speed) * Time.deltaTime), Space.World);
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
        if(pointsToGiveToPlayer != 0)
        {
            var projectile = instigator.GetComponent<Projectile>();
            if(projectile != null && projectile.Owner.GetComponent <Player>() != null)
            {
                GameManager.Instance.AddPoints(pointsToGiveToPlayer);
                FloatingText.Show(string.Format("+{0}", pointsToGiveToPlayer), "PointStarText", new FromWorldPointTextPositioner(Camera.main, transform.position, 1.5f, 50));
            }
        }

        DestroyProjectile();
    }

    protected override void OnCollideOther(Collider2D coll)
    {
        DestroyProjectile();
    }

    protected override void OnCollideTakeDamage(Collider2D coll, ITakeDamage takeDamage)
    {
        takeDamage.TakeDamage(damage, gameObject);
        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        if (destroyedEffect != null)
            Instantiate(destroyedEffect, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
