using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    [SerializeField] protected float _projectileDamage;

    [SerializeField] protected FightingUnit _owner;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        Vector2Int gridPos = World.singleton.WorldToGrid(transform.position);
        List<Entity> entities = World.singleton.GetNearestEntites(gridPos.x, gridPos.y);

        foreach (var e in entities)
        {
            if (e is FightingUnit fu && fu.Team == _owner.Team) continue;

            if (e.CurHealth > 0 && Vector3.Distance(e.transform.position, transform.position) <= e.Radius + Radius)
            {
                e.DealDamage(_projectileDamage);
                Destroy(gameObject);
            }
        }
    }

    public void Init(FightingUnit owner)
    {
        _owner = owner;
    }
}
