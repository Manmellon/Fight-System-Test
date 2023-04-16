using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeUnit : FightingUnit
{
    [SerializeField] protected Projectile _projectilePrefab;

    public override void Attack()
    {
        base.Attack();

        Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.Euler(transform.forward));
        projectile.Init(this);
    }
}
