using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public class FightingUnit : Entity
{
    [SerializeField] protected int _team;
    public int Team => _team;

    [SerializeField] protected float _attackCooldown;
    protected float _prevAttackTime;

    [SerializeField] protected float _attackRange;

    [SerializeField] protected float randomMovementRadius = 5;

    protected Entity attackTarget;
    protected Vector3 movementTarget;

    protected List<Vector3> wayPoints = new List<Vector3>();

    [SerializeField] protected float _stoppingDistance = 0.1f;

    void Start()
    {
        
    }

    void Update()
    {
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

            if (Vector3.Distance(transform.position, movementTarget) <= _attackRange)
            {
                transform.LookAt(attackTarget.transform);
                Attack();
                wayPoints.Clear();
                return;
            }
            //Move to enemy
            else
            {
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

        if (Vector3.Distance(transform.position, movementTarget) <= _stoppingDistance)
        {
            wayPoints.Clear();
        }
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

        JumpPointParam jpParam = new JumpPointParam(World.singleton.SearchGrid, new GridPos(gridPos.x, gridPos.y), new GridPos(targetGridPos.x, targetGridPos.y), 
                                                    EndNodeUnWalkableTreatment.ALLOW, DiagonalMovement.IfAtLeastOneWalkable);
        List<GridPos> gridPositions = JumpPointFinder.FindPath(jpParam);

        foreach (var gp in gridPositions)
        {
            result.Add( World.singleton.GridToWorld(gp.x, gp.y) );
        }

        return result;
    }
}
