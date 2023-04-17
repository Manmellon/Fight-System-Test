using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public class Chunk
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
    public StaticGrid SearchGrid => searchGrid;

    [SerializeField] private int width = 80;
    [SerializeField] private int height = 80;

    [Range(0, 1)]
    [SerializeField] private float obstaclesDensity = 0.1f;

    [SerializeField] private List<Obstacle> obstaclePrefabs;
    [SerializeField] private Transform obstaclesParent;

    [SerializeField] private int startUnitsCount = 20;

    [SerializeField] private List<FightingUnit> unitsPrefabs;
    [SerializeField] private Transform unitsParent;

    private Chunk[,] chunks;

    public static World singleton { get; private set; }

    private void Awake()
    {
        if (singleton == null) singleton = this;
    }

    void Start()
    {
        Init();
    }

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
                    if (gridPos.x != j || gridPos.y != i)
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
                    //If outside of world, kill entity
                    if (ep.gridPos.x < 0 || ep.gridPos.x >= width || ep.gridPos.y < 0 || ep.gridPos.y >= height)
                    {
                        ep.entity.OnDeath();
                    }
                    else
                    {
                        chunks[ep.gridPos.y, ep.gridPos.x].entities.Add(ep.entity);
                    }
                }
            }
        }
    }

    public void Init()
    {
        chunks = new Chunk[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                chunks[i, j] = new Chunk();
                chunks[i, j].entities = new List<Entity>();
            }
        }

        searchGrid = new StaticGrid(width, height);

        List<Vector3> freePlacesForSpawn = new List<Vector3>();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                float r = UnityEngine.Random.Range(0.0f, 1.0f);
                bool hasObstacle = r < obstaclesDensity || obstaclesDensity >= 1.0f;

                searchGrid.SetWalkableAt(j, i, !hasObstacle);

                if (hasObstacle)
                {
                    Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)], GridToWorld(new Vector2Int(j, i)), Quaternion.identity, obstaclesParent);
                }
                else
                {
                    freePlacesForSpawn.Add( GridToWorld(new Vector2Int(j, i)) );
                }
            }
        }

        for (int i = 0; i < startUnitsCount; i++)
        {
            int place_idx = Random.Range(0, freePlacesForSpawn.Count);
            Instantiate(unitsPrefabs[Random.Range(0, unitsPrefabs.Count)], freePlacesForSpawn[place_idx], Quaternion.identity, unitsParent);
            freePlacesForSpawn.RemoveAt(place_idx);
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

    public List<Entity> GetAllEntities()
    {
        List<Entity> entities = new List<Entity>();

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                entities.AddRange(chunks[i, j].entities);
            }
        }

        return entities;
    }

    //Get entities from current chunk, and all neighbours chunks
    public List<Entity> GetNearestEntites(int gridX, int gridY)
    {
        if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height) return null;

        List<Entity> entities = new List<Entity>();

        if (gridX > 0)
        {
            if (gridY > 0)
                entities.AddRange(chunks[gridY - 1, gridX - 1].entities);

            entities.AddRange(chunks[gridY, gridX - 1].entities);

            if (gridY < height - 1)
                entities.AddRange(chunks[gridY + 1, gridX - 1].entities);
        }

        if (gridX < width - 1)
        {
            if (gridY > 0)
                entities.AddRange(chunks[gridY - 1, gridX + 1].entities);

            entities.AddRange(chunks[gridY, gridX + 1].entities);

            if (gridY < height - 1)
                entities.AddRange(chunks[gridY + 1, gridX + 1].entities);
        }

        if (gridY > 0)
            entities.AddRange(chunks[gridY - 1, gridX].entities);

        entities.AddRange(chunks[gridY, gridX].entities);

        if (gridY < height - 1)
            entities.AddRange(chunks[gridY + 1, gridX].entities);

        return entities;
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x - width / 2 + 0.5f, 0, gridPos.y - height / 2 + 0.5f);
    }

    public Vector2Int WorldToGrid(Vector3 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x + width / 2), Mathf.FloorToInt(pos.z + height / 2));
    }
}
