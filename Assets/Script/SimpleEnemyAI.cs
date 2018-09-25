using UnityEngine;
using System.Collections;
using System;

public class SimpleEnemyAI : MonoBehaviour, ITakeDamage, IPlayerRespawnListener {

    public float Speed;
    public bool SeenPlayer = false;
    public float FireRate;
    public Projectile Projectile;
    public GameObject DestroyedEffect;
   // public int PointsToGivePlayer;
    private CharController2D _controller;
    private Vector2 _direction;
    private Vector2 _startPosition;
    private float _canFireIn;

    public void Start()
    {
        _controller = GetComponent<CharController2D>();
        _direction = new Vector2(-1, 0);
        _startPosition = transform.position;
    }

    public void Update()
    {
        if(SeenPlayer)
            _controller.SetHForce(_direction.x * 0);

        _controller.SetHForce(_direction.x * Speed);
        if ((_direction.x < 0 && _controller.state.IsCollidingLeft) || (_direction.x>0 && _controller.state.IsCollidingRight))
        {
            _direction = -_direction;
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        if ((_canFireIn -= Time.deltaTime) > 0)
            return;

        var raycast = Physics2D.Raycast(transform.position,_direction, 20, 1 << LayerMask.NameToLayer("Player"));
        if (!raycast)
            return;
        else {
            var projectile = (Projectile)Instantiate(Projectile, transform.position, transform.rotation);
            SeenPlayer = true;
            projectile.Initialize(gameObject, _direction, _controller.Velocity);
            _canFireIn = FireRate;
        }
    }

    public void TakeDamage(int damage, GameObject instigator)
    {
       
        Instantiate(DestroyedEffect, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    public void OnPlayerRespawnInThisCheckpoint(Checkpoint checkpoint, Player player)
    {
        _direction = new Vector2(-1, 0);
        transform.localScale = new Vector3(1, 1, 1);
        transform.position = _startPosition;
        gameObject.SetActive(true);
    }
}
