using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightingUnit : Entity
{
    [SerializeField] protected int _team;
    public int Team => _team;

    [SerializeField] protected float _attackCooldown;
    protected float _prevAttackTime;

    [SerializeField] protected float _attackRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int team)
    {
        _team = team;
    }

    public virtual void Attack()
    {
        _prevAttackTime = Time.time;

        if (_animator)
            _animator.Play("Attack");
    }

    public Entity FindNearestEnemy()
    {
        Entity result = null;
        float minDistance = 1_000_000_000;

        List<Entity> entities = World.singleton.GetAllEntities();

        foreach (var e in entities)
        {
            if (e is FightingUnit fu && fu.Team != Team && 
                (result == null || Vector3.Distance(transform.position, e.transform.position) < minDistance) 
                )
            {
                result = e;
                minDistance = Vector3.Distance(transform.position, result.transform.position)
            }
        }

        return result;
    }
}
