using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Animator _animator;
    [SerializeField] ParticleSystem _deathParticles;

    [Header("Settings")]
    [SerializeField] private float _radius;
    public float Radius => _radius;

    [SerializeField] private float movingSpeed;

    [SerializeField] private float _maxHealth;
    public float MaxHealth => _maxHealth;

    [SerializeField] private float _currentHealth;
    public float CurHealth => _currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        World.singleton.AddEntity(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * movingSpeed;
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
