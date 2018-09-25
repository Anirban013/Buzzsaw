using UnityEngine;
using System.Collections;

public abstract class Projectile : MonoBehaviour {

    public float speed;
    public LayerMask collisionMask;
    public GameObject Owner { get; private set; }
    public Vector2 Direction { get; private set; }
    public Vector2 InitialVelocity { get; private set; }

    public void Initialize(GameObject owner, Vector2 direction, Vector2 initialVelocity)
    {
        transform.right = direction;
        Owner = owner;
        Direction = direction;
        InitialVelocity = initialVelocity;
        OnInitialized();
    }

    protected virtual void OnInitialized()
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D coll)
    {
        if((collisionMask.value & (1 << coll.gameObject.layer)) == 0)
        {
            OnNotCollideWith(coll);
            return;
        }
        var isOwner = coll.gameObject == Owner;
        if (isOwner)
        {
            OnCollideOwner();
            return;
        }

        var takeDamage = (ITakeDamage)coll.GetComponent(typeof(ITakeDamage));
        if(takeDamage != null)
        {
            OnCollideTakeDamage(coll, takeDamage);
            return;
        }

        OnCollideOther(coll);
    }

    protected virtual void OnNotCollideWith(Collider2D coll)
    {

    }

    protected virtual void OnCollideOwner()
    {

    }

    protected virtual void OnCollideTakeDamage(Collider2D coll, ITakeDamage takeDamage)
    {

    }
    protected virtual void OnCollideOther(Collider2D coll)
    {

    }
}
