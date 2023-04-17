using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    [SerializeField] protected float _projectileDamage;

    [SerializeField] protected FightingUnit _owner;

    protected override void Update()
    {
        base.Update();

        Vector2Int gridPos = World.singleton.WorldToGrid(transform.position);
        List<Entity> entities = World.singleton.GetNearestEntites(gridPos.x, gridPos.y);

        foreach (var e in entities)
        {
            if (e is FightingUnit fu && fu.Team == _owner.Team) continue;

            Vector2 my_2d_pos = new Vector2(transform.position.x, transform.position.z);
            Vector2 e_2d_pos = new Vector2(e.transform.position.x, e.transform.position.z);
            if (e.CurHealth > 0 && Vector2.Distance(my_2d_pos, e_2d_pos) <= e.Radius + Radius)
            {
                e.DealDamage(_projectileDamage);
                OnDeath();
            }
        }
    }

    public void Init(FightingUnit owner)
    {
        _owner = owner;
    }
}
