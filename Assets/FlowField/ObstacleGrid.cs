using System;
using System.Collections.Generic;
using UnityEngine;

/// Returns an "Obstacle Grid" --
/// a 2D array of integers with either NULL or MAXINT
/// NULL - empty space
/// MAXINT - impassable space
public class ObstacleGrid
{
    private readonly Dictionary<Vector2Int, int> dict;
    private readonly int col;
    private readonly int row;
    private readonly CustomGrid cg;
    private Vector2Int bl; // bottom left corner of the grid
    private readonly RaycastHit[] hitResult = new RaycastHit[1];

    public ObstacleGrid(int col, int row, CustomGrid cg, Vector2Int bl)
    {
        this.col = col;
        this.row = row;
        this.cg = cg;
        this.bl = bl;
        dict = new Dictionary<Vector2Int, int>(row * col);
    }

    public static ValueTuple<Vector3, Vector3> GetBounds(Vector3 bounds1, Vector3 bounds2)
    {

        Vector3 bottomLeft = Vector2.zero;
        Vector3 topRight = Vector2.zero;

        // we want to get the top-right and bottom-left corners of a box given any bounds
        if (bounds1.x < bounds2.x)
        {
            bottomLeft.x = bounds1.x;
            topRight.x = bounds2.x;
        }
        else
        {
            bottomLeft.x = bounds2.x;
            topRight.x = bounds1.x;
        }

        if (bounds1.z < bounds2.z)
        {
            bottomLeft.z = bounds1.z;
            topRight.z = bounds2.z;
        }
        else
        {
            bottomLeft.z = bounds2.z;
            topRight.z = bounds1.z;
        }

        return new ValueTuple<Vector3, Vector3>(bottomLeft, topRight);
    }

    //This function will generate a 2D array of either NULL or MAXINT depending on whether there are
    //obstacles in the way or not.

    public Dictionary<Vector2Int, int> GenerateBlockedDictionary(int obstacleMask)
    {
        var cellSize = cg.cellSize;
        var radius = cellSize * 0.5f; //radius of capsulecast
        var offset = 0.5f * cellSize * (Vector3.right + Vector3.forward) + (1000.0f * Vector3.up);

        var offsetRow = (int)offset.z;
        var offsetCol = (int)offset.x;

        dict.Clear();
        // every column is X direction

        for (int c = bl.c; c < col; c++)
        {
            // every row is Z direction
            for (int r = bl.r; r < row; r++)
            {
                // reduce CPU consumption of function calls
                // Vector3 origin = cg.CellToWorld(r, c) + offset;
                var origin = new Vector3(r * cellSize + offsetRow, 0, c * cellSize + offsetCol);
                // var origin = new Vector3(c * cellSize + offsetCol, 0, r * cellSize + offsetRow);

                //check for obstacle
                if (Physics.SphereCastNonAlloc(origin, radius, Vector3.down, hitResult, 50000.0f, 1 << obstacleMask) > 0)
                {
                    dict[new Vector2Int(r, c, row)] = int.MaxValue; //Max value represents impassable object
                }
            }
        }

        return dict;
    }
}