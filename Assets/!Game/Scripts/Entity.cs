using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected Animator _animator;

    [SerializeField] protected ParticleSystem _deathParticles;

    [Header("Settings")]
    [SerializeField] protected float _radius;
    public float Radius => _radius;

    [SerializeField] protected float _movingSpeed;
    public float MovingSpeed => _movingSpeed;

    [SerializeField] protected float _maxHealth;
    public float MaxHealth => _maxHealth;

    [SerializeField] protected float _currentHealth;
    public float CurHealth => _currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;

        World.singleton.AddEntity(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * _movingSpeed * Time.fixedDeltaTime;
    }

    public void DealDamage(float damage)
    {
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, _maxHealth);

        if (_currentHealth <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        if (_animator)
            _animator.Play("Death");

        if (_deathParticles)
            _deathParticles.Play();
    }
}
