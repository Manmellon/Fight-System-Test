using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EpPathFinding.cs;

public class World : MonoBehaviour
{
    private StaticGrid searchGrid;

    [SerializeField] private int width = 80;
    [SerializeField] private int height = 80;

    [Range(0, 1)]
    [SerializeField] private float obstaclesDensity = 0.1f;

    [SerializeField] private List<GameObject> obstaclePrefabs;

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
        
    }

    public void Init()
    {
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

    public Vector3 GridToWorld(int x, int y)
    {
        return new Vector3(x - width / 2 + 0.5f, 0, y - height / 2 + 0.5f);
    }
}
