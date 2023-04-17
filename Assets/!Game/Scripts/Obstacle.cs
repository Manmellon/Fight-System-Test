using UnityEngine;

public class Obstacle : Entity
{
    public override void OnDeath()
    {
        Vector2Int gridPos = World.singleton.WorldToGrid(transform.position);
        World.singleton.SearchGrid.SetWalkableAt(gridPos.x, gridPos.y, true);

        base.OnDeath();
    }
}
