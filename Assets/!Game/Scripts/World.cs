using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public struct Chunk
{
    public List<Entity> entities;
}

//Struct to store entity pos to not calc it 2 times in update
public struct EntityWithPos
{
    public Entity entity;
    public Vector2Int gridPos;
}

public class World : MonoBehaviour
{
    private StaticGrid searchGrid;

    [SerializeField] private int width = 80;
    [SerializeField] private int height = 80;

    [Range(0, 1)]
    [SerializeField] private float obstaclesDensity = 0.1f;

    [SerializeField] private List<Obstacle> obstaclePrefabs;

    private Chunk[,] chunks;

    public static World singleton { get; private set; }

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                List<EntityWithPos> updatedEntities = new List<EntityWithPos>();

                //Create list of entites changing chunk
                for (int k = 0; k < chunks[i, j].entities.Count; k++)
                {
                    Vector2Int gridPos = WorldToGrid(chunks[i, j].entities[k].transform.position);
                    if (gridPos.x != j && gridPos.y != i)
                    {
                        EntityWithPos ep = new EntityWithPos();
                        ep.entity = chunks[i, j].entities[k];
                        ep.gridPos = gridPos;
                        updatedEntities.Add(ep);
                    }
                }

                //Remove from old chunks
                foreach (var ep in updatedEntities)
                {
                    chunks[i, j].entities.Remove(ep.entity);
                }

                //Add to new chunk
                foreach (var ep in updatedEntities)
                {
                    chunks[ep.gridPos.y, ep.gridPos.x].entities.Add(ep.entity);
                }
            }
        }
    }

    public void Init()
    {
        chunks = new Chunk[height, width];

        searchGrid = new StaticGrid(width, height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float r = Random.Range(0.0f, 1.0f);
                bool hasObstacle = r < obstaclesDensity || obstaclesDensity >= 1.0f;

                searchGrid.SetWalkableAt(j, i, hasObstacle);

                if (hasObstacle)
                    Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], GridToWorld(j, i), Quaternion.identity);
            }
        }
    }

    public void AddEntity(Entity entity)
    {
        Vector2Int gridPos = WorldToGrid(entity.transform.position);
        chunks[gridPos.y, gridPos.x].entities.Add(entity);
    }

    public void RemoveEntity(Entity entity)
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                chunks[i, j].entities.Remove(entity);
            }
        }
    }

    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x - width / 2 + 0.5f, 0, y - height / 2 + 0.5f);
    }

    public Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x - 0.5f + width / 2), Mathf.FloorToInt(pos.y - 0.5f + height / 2));
    }
}
