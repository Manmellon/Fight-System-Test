using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : FightingUnit
{
    [SerializeField] protected float _meleeDamage;

    //We use it to damage only targets in front, and not behind of attacking unit
    [SerializeField] protected float _meleeAttackCenterDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Attack()
    {
        base.Attack();

        Vector2Int gridPos = World.singleton.WorldToGrid(transform.position);
        List<Entity> entities = World.singleton.GetNearestEntites(gridPos.x, gridPos.y);

        foreach (var e in entities)
        {
            if (e is FightingUnit fu && fu.Team == Team) continue;

            if (e.CurHealth > 0 && Vector3.Distance(e.transform.position, transform.position + transform.forward * _meleeAttackCenterDistance) <= e.Radius + _attackRange)
            {
                e.DealDamage(_meleeDamage);
            }
        }
    }
}
