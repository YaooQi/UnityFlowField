using System.Collections.Generic;
using UnityEngine;

// Static class called for Flowfield generation
public class FlowFieldProvider
{
    private bool dynamicObstacles;
    private int obstacleMask;
    private readonly Dictionary<Vector2Int, Vector3> flowFieldMap;

    // Utility used for transforming between world and grid coordinates
    private readonly CustomGrid cg;
    private Dictionary<Vector2Int, int> obstacles = new();

    private Vector2Int b1;
    private Vector2Int b2;

    private readonly int col;
    private readonly int row;

    private readonly DijkstraGrid dg;
    private readonly ObstacleGrid og;
    private readonly FlowField ff;


    public FlowFieldProvider(float cellSize, Vector3 bounds1, Vector3 bounds2)
    {
        cg = new CustomGrid(cellSize);

        var newBounds = ObstacleGrid.GetBounds(bounds1, bounds2);

        b1 = cg.WorldToCell(newBounds.Item1, row);
        b2 = cg.WorldToCell(newBounds.Item2, row);

        col = b2.c - b1.c; // x length
        row = b2.r - b1.r; // z length

        dg = new DijkstraGrid(col, row);
        og = new ObstacleGrid(col, row, cg, b1);
        ff = new FlowField(col, row);
        flowFieldMap = new Dictionary<Vector2Int, Vector3>(col * row);
    }

    public void SetObstracle(int obstacleMask, bool dynamicObstacles)
    {
        this.obstacleMask = obstacleMask;
        this.dynamicObstacles = dynamicObstacles;
        obstacles = og.GenerateBlockedDictionary(obstacleMask);
    }

    public void GenerateNewField(Vector3 dest)
    {
        if (dynamicObstacles)
        {
            obstacles = og.GenerateBlockedDictionary(obstacleMask);
        }
        GenerateFlowField(dest);
    }

    public Vector3 GetVector(Vector3 position)
    {
        if (flowFieldMap.ContainsKey(cg.WorldToCell(position, row)))
        {
            return flowFieldMap[cg.WorldToCell(position, row)];
        }
        return Vector3.zero;
    }

    public Dictionary<Vector2Int, Vector3> GetFlowField()
    {
        return flowFieldMap;
    }

    public Dictionary<Vector2Int, int> GetObstacles()
    {
        return obstacles;
    }

    public CustomGrid GetGrid()
    {
        return cg;
    }

    private void GenerateFlowField(Vector3 destination)
    {
        var rOff = b1.c;
        var cOff = b1.r;

        // 2. Flood fill grid to get cost values for each square
        var dGrid = dg.GenerateGrid(col, row, b1, obstacles, cg.WorldToCell(destination, row));

        // 3. Generate FlowField array from grid
        var flowFieldArray = ff.Generate(dGrid);

        // 4. Transfer Array to Dictionary for ease of Querying
        flowFieldMap.Clear();

        for (int i = 0; i < flowFieldArray.GetLength(0); i++)
        {
            for (int j = 0; j < flowFieldArray.GetLength(1); j++)
            {
                Vector2Int index = new(i + cOff, j + rOff, row);
                flowFieldMap[index] = flowFieldArray[i, j];
            }
        }
    }
}