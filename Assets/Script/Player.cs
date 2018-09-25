using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ITakeDamage {

	private bool _isFacigR;
	private CharController2D _cntrl;
	private float _normalHSpeed;
    private float _canFireIn;

    public int Health { get; private set; }
    public GameObject fireProjectileEffect;
    public int maxHealth = 100;
	public float mSpeed = 8f;
	public float accOnGrnd;
	public float accInAir;
    public bool IsDead { get; private set; }
    public GameObject OuchEffect;
    public Projectile _projectile;
    public Transform ProjectileFireLocation;
    public float fireRate;

	public void Awake(){
		_cntrl = GetComponent<CharController2D> ();
		_isFacigR = transform.localScale.x > 0;
        Health = maxHealth;
	}

	public void Update(){

        _canFireIn -= Time.deltaTime;
        if(!IsDead)
		HandleInput ();

		float mvmntFactor = _cntrl.state.IsGrounded ? accOnGrnd : accInAir;
		//Debug.Log ("fac " + mvmntFactor);

		float speed = Mathf.Lerp (_cntrl.Velocity.x, (_normalHSpeed * mSpeed), (Time.deltaTime * mvmntFactor));
        //Debug.Log ("Speed " + speed);

        if (IsDead)
            _cntrl.SetHForce(0);
        else
		    _cntrl.SetHForce (speed);
	}

	public void Kill(){
        _cntrl.HandleCollisions = false;
        GetComponent<Collider2D>().enabled = false;
        IsDead = true;
        Health = 0;
        _cntrl.SetForce(new Vector2(0, 20));
	}

	public void RespawnAt(Transform spawnPoint){
        if (!_isFacigR)
            Flip();

        IsDead = false;
        GetComponent<Collider2D>().enabled = true;
        _cntrl.HandleCollisions = true;
        Health = maxHealth;
        transform.position = spawnPoint.position;
    }

    public void TakeDamage(int damage, GameObject instigate)
    {
        FloatingText.Show(string.Format("-{0}", damage), "PlayerDamageText", new FromWorldPointTextPositioner(Camera.main, transform.position, 2f, 60));
        Instantiate(OuchEffect, transform.position, transform.rotation);
        Health -= damage;

        if (Health <= 0)
            LevelManager.Instance.KillPlayer();
    }

	public void HandleInput(){
		if (Input.GetKey (KeyCode.D)) {
			_normalHSpeed = 1;
			//_cntrl.SetForce (new Vector2 (-1f, 0));
			if (!_isFacigR) {
				Flip ();
				_cntrl._isFacingRight = true;
			}
		} else if (Input.GetKey (KeyCode.A)) {
			_normalHSpeed = -1;
			if (_isFacigR) {
				Flip ();
				_cntrl._isFacingRight = false;
			}
		} 
		else {
			_normalHSpeed = 0;
		}
		if (_cntrl.CanJump && Input.GetKey (KeyCode.Space)) {
			Debug.Log ("Jump");
			_cntrl.Jump();
		}

        if (Input.GetMouseButtonDown(0))
        {
            FireProjectile();
        }

      

	}
    private void FireProjectile()
    {
        if(fireProjectileEffect != null)
        {
           var effect =  (GameObject)Instantiate(fireProjectileEffect, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
            effect.transform.parent = transform;
        }
        if (_canFireIn > 0)
            return;

        var direction = _isFacigR ? Vector2.right : -Vector2.right;

        var projectile = (Projectile)Instantiate(_projectile, ProjectileFireLocation.position, ProjectileFireLocation.rotation);
        projectile.Initialize(gameObject, direction, _cntrl.Velocity);

        //projectile.transform.localScale = new Vector3(_isFacigR ? 1 : -1, 1, 1);
        _canFireIn = fireRate;
    }
	private void Flip(){
		transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
		_isFacigR = transform.localScale.x > 0;
	}
}
