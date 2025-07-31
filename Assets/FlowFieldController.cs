using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// Script used to access the static FlowFieldProvider class
[ExecuteAlways]
public class FlowFieldController : MonoBehaviour
{

    [Header("Field generation settings")]
    // determines whether or not we will re-check for obstacles
    public bool dynamicObstacles;

    public float cellSize;

    public Vector3 b1;
    public Vector3 b2;

    public int obstacleLayer;

    public GameObject player;

    [Header("Debug Options")]
    public bool showObstacles;
    public bool showDijkstra;
    public bool showVectors;

    // Dictionary linking grid coordinates to passability values
    // Dictionary<Vector2Int, int> obstacles;

    // Dictionary linking grid coordinates to Vector3s of FlowField
    [Header("Resource Allocation")]
    public float refreshRate;
    float timer;

    // changes  --------------
    FlowFieldProvider ffp;

    // Start is called before the first frame update
    void Awake()
    {
        timer = refreshRate;

        ffp = new FlowFieldProvider(cellSize, b1, b2);
        ffp.SetObstracle(obstacleLayer, dynamicObstacles);
        ffp.UpdateFlowField(player.transform.position);
    }

    void LateUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = refreshRate;
            ffp?.UpdateFlowField(player.transform.position);
        }
    }

    void OnDrawGizmos()
    {
        ffp ??= new FlowFieldProvider(cellSize, b1, b2);
        ffp.SetObstracle(obstacleLayer, dynamicObstacles);

        var cg = ffp.GetGrid();
        // show blocked squares
        if (showObstacles)
        {
            Gizmos.color = new Color(1, 0.5f, 0.5f, 0.5f);

            var obstacles = ffp.GetObstacles();
            foreach (KeyValuePair<Vector2Int, int> pair in obstacles)
            {
                if (pair.Value == int.MaxValue)
                {
                    Gizmos.DrawCube(cg.TupleToWorld(pair.Key) + (Vector3.forward * cellSize / 2) + (Vector3.right * cellSize / 2), new Vector3(cellSize, cellSize, cellSize));
                }
            }
        }

        // TODO: SAVE DIJKSTRA GRID IN A STATIC CLASS TO ACCESS FOR DEBUG DRAWING
        // show dijkstra values
        // if(showDijkstra){
        //     for (int i = 0; i < dGrid.GetLength(0); i++){
        //         for(int j = 0; j < dGrid.GetLength(1); j++){
        //             string val = dGrid[i,j].ToString();
        //             UnityEditor.Handles.Label(new Vector3((i - 10) * 5, 1.0f, (j - 20) * 5), val);
        //         }
        //     }
        // }

        // show vectors
        if (showVectors)
        {
            var ff = ffp.GetFlowField();
            Gizmos.color = new Color(1, 1.0f, 1.0f, 1.0f);
            foreach (KeyValuePair<Vector2Int, Vector3> pair in ff)
            {
                DrawArrow.ForDebug(cg.TupleToWorld(pair.Key) + (Vector3.forward * cellSize / 2) + (Vector3.right * cellSize / 2), pair.Value);
            }
        }
    }
}
