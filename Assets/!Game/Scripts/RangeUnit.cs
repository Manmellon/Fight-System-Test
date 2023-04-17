using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUnit : FightingUnit
{
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] protected Transform shootPlace;

    protected override void Attack()
    {
        base.Attack();

        Projectile projectile = Instantiate(_projectilePrefab, shootPlace.position, transform.rotation);
        projectile.Init(this);
    }
}
