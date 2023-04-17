using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public class FightingUnit : Entity
{
    [SerializeField] protected int _team;
    public int Team => _team;

    [SerializeField] protected float _attackCooldown;
    [SerializeField] protected float _prevAttackTime;

    [SerializeField] protected float _attackRange;

    [SerializeField] protected float randomMovementRadius = 5;

    [SerializeField] protected Entity attackTarget;
    [SerializeField] protected Vector3 movementTarget;

    [SerializeField] protected List<Vector3> wayPoints = new List<Vector3>();

    [SerializeField] protected float _stoppingDistance = 0.1f;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        attackTarget = FindNearestEnemy();

        if (attackTarget == null)
        {
            if (wayPoints.Count == 0)
            {
                //Choose random destination
                Vector2 randomPointInCircle = Random.insideUnitCircle * randomMovementRadius;
                movementTarget = transform.position + new Vector3(randomPointInCircle.x, 0, randomPointInCircle.y);
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
                _currentSpeed = 0;
                return;
            }
            else
            {
                //Move to enemy
                wayPoints = GenerateWayPoints(movementTarget);
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
        _currentSpeed = _movingSpeed;

        if (Vector3.Distance(transform.position, movementTarget) <= _stoppingDistance)
        {
            wayPoints.Clear();
        }
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
            result.Add( World.singleton.GridToWorld(gp.x, gp.y) );
        }

        return result;
    }

    public virtual bool IsAggro()
    {
        return Vector3.Distance(transform.position, movementTarget) <= _attackRange;
    }
}
