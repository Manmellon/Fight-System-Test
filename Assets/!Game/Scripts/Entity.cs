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

    protected float _currentSpeed;

    [SerializeField] protected float _maxHealth;
    public float MaxHealth => _maxHealth;

    protected float _currentHealth;
    public float CurHealth => _currentHealth;

    protected virtual void Start()
    {
        _currentHealth = _maxHealth;
        _currentSpeed = _movingSpeed;

        World.singleton.AddEntity(this);
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        transform.position += transform.forward * _currentSpeed * Time.fixedDeltaTime;
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

        World.singleton.RemoveEntity(this);
        Destroy(gameObject);
    }
}
