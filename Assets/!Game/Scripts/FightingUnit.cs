using System;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

[Serializable]
public class SerializedFightingUnit : SerializedEntity
{
    public int team;
    public float attackDamage;
    public float attackRange;
    public float attackCooldown;

    public SerializedFightingUnit(string name, float movingSpeed, float maxHealth, int team, float attackDamage, float attackRange, float attackCooldown) : base(name, movingSpeed, maxHealth)
    {
        this.team = team;
        this.attackDamage = attackDamage;
        this.attackRange = attackRange;
        this.attackCooldown = attackCooldown;
    }
}

public class FightingUnit : Entity
{
    [SerializeField] protected int _team;
    public int Team => _team;

    [SerializeField] protected float _attackDamage;
    public float AttackDamage => _attackDamage;

    [SerializeField] protected float _attackCooldown;
    protected float _prevAttackTime;

    [SerializeField] protected float _attackRange;

    [SerializeField] protected float randomMovementRadius = 5;

    protected Entity attackTarget;
    protected Vector3 movementTarget;

    protected List<Vector3> wayPoints = new List<Vector3>();

    [SerializeField] protected float _stoppingDistance = 0.1f;

    private float searchTargetCooldown = 1.0f;
    private float prevSearchTime;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        attackTarget = FindNearestEnemy();

        _currentSpeed = 0;

        if (attackTarget == null)
        {
            if (wayPoints.Count == 0)
            {
                //Choose random destination
                Vector2 randomPointInCircle = UnityEngine.Random.insideUnitCircle * randomMovementRadius;
                movementTarget = transform.position + new Vector3(randomPointInCircle.x, 0, randomPointInCircle.y);
                movementTarget = World.singleton.GridToWorld(World.singleton.WorldToGrid(movementTarget));
                wayPoints = GenerateWayPoints(movementTarget);
            }
        }
        else
        {
            movementTarget = attackTarget.transform.position;

            if (IsAggro())
            {
                transform.LookAt(attackTarget.transform);
                TryAttack();
                wayPoints.Clear();
                return;
            }
            else
            {
                //Move to enemy
                //Regenerate way costs too much, so we will do it with cooldown
                if (Time.time > prevSearchTime + searchTargetCooldown)
                {
                    prevSearchTime = Time.time;
                    wayPoints = GenerateWayPoints(movementTarget);
                }
            }
        }

        if (wayPoints.Count == 0) return;

        Vector3 currentMovementTarget;
        if (wayPoints.Count == 1) //When we are already in target cell
        {
            currentMovementTarget = wayPoints[0];
        }
        else
        {
            currentMovementTarget = wayPoints[1];
        }

        transform.LookAt(currentMovementTarget);

        Vector2 my_2d_pos = new Vector2(transform.position.x, transform.position.z);
        Vector2 target_2d_pos = new Vector2(currentMovementTarget.x, currentMovementTarget.z);
        if (Vector2.Distance(my_2d_pos, target_2d_pos) <= _stoppingDistance)
        {
            wayPoints.RemoveAt(0);
            return;
        }

        _currentSpeed = _movingSpeed;
    }

    public void Init(int team)
    {
        _team = team;
    }

    public void TryAttack()
    {
        if (CanAttack())
            Attack();
    }

    public bool CanAttack()
    {
        return Time.time >= _prevAttackTime + _attackCooldown;
    }

    protected virtual void Attack()
    {
        _prevAttackTime = Time.time;

        if (_animator)
            _animator.Play("Attack");
    }

    public Entity FindNearestEnemy()
    {
        List<FightingUnit> fightingUnits = World.singleton.GetFightingUnits(_team == 0 ? 1 : 0);
        if (fightingUnits == null) return null;

        Entity result = null;
        float minDistance = 1_000_000_000;

        foreach (var fu in fightingUnits)
        {
            if (Vector3.Distance(transform.position, fu.transform.position) < minDistance)
            {
                result = fu;
                minDistance = Vector3.Distance(transform.position, result.transform.position);
            }
        }

        return result;
    }

    public List<Vector3> GenerateWayPoints(Vector3 target)
    {
        List<Vector3> result = new List<Vector3>();

        Vector2Int gridPos = World.singleton.WorldToGrid(transform.position);
        Vector2Int targetGridPos = World.singleton.WorldToGrid(target);

        StaticGrid searchGrid = new StaticGrid(World.singleton.SearchGrid); //We need copy, because FindPath modify grid...

        JumpPointParam jpParam = new JumpPointParam(searchGrid, new GridPos(gridPos.x, gridPos.y), new GridPos(targetGridPos.x, targetGridPos.y), 
                                                    EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.IfAtLeastOneWalkable);
        List<GridPos> gridPositions = JumpPointFinder.FindPath(jpParam);

        foreach (var gp in gridPositions)
        {
            result.Add( World.singleton.GridToWorld( new Vector2Int(gp.x, gp.y) ) );
        }

        return result;
    }

    public virtual bool IsAggro()
    {
        return Vector3.Distance(transform.position, movementTarget) <= _attackRange;
    }

    public override void OnDeath()
    {
        World.singleton.RemoveFightingUnit(this);

        base.OnDeath();
    }

    public override SerializedEntity Serialize()
    {
        SerializedFightingUnit serializedFightingUnit = new SerializedFightingUnit(name, _movingSpeed, _maxHealth, _team, _attackDamage, _attackRange, _attackCooldown);
        return serializedFightingUnit;
    }

    public override void Deserialize(SerializedEntity serializedEntity)
    {
        SerializedFightingUnit serializedFightingUnit = (SerializedFightingUnit)serializedEntity;
        _team = serializedFightingUnit.team;
        _attackDamage = serializedFightingUnit.attackDamage;
        _attackRange = serializedFightingUnit.attackRange;
        _attackCooldown = serializedFightingUnit.attackCooldown;
    }
}
