using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Entity
{
    [SerializeField] private float _projectileDamage;

    [SerializeField] private FightingUnit _owner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
