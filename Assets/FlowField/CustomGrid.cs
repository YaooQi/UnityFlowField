using System;
using UnityEngine;

/// An Object with utility functions that help
/// with discretizing world space
public class CustomGrid
{
    //grid size
    public float cellSize;

    public CustomGrid(float cellSize)
    {
        this.cellSize = cellSize;
    }

    //transforms world position into grid indices
    public Vector2Int WorldToCell(Vector3 worldPos, int row)
    {
        int xCell = (int)Mathf.Floor(worldPos.x / cellSize);
        int zCell = (int)Mathf.Floor(worldPos.z / cellSize);

        return new Vector2Int(xCell, zCell, row);
    }

    public bool Equals(Vector2Int a, Vector2Int b)
    {
        return a.hashCode == b.hashCode;
    }

    public bool Equals(Vector3 a, Vector3 b)
    {
        if (a.x == b.x && a.z == b.z)
        {
            return true;
        }

        int xa = (int)Mathf.Floor(a.x / cellSize);
        int xb = (int)Mathf.Floor(b.x / cellSize);
        int za = (int)Mathf.Floor(a.z / cellSize);
        int zb = (int)Mathf.Floor(b.z / cellSize);
        return xa == xb && za == zb;
    }

    //turns grid indices into approximate world coordinates
    public Vector3 CellToWorld(int xCell, int zCell)
    {
        return new(xCell * cellSize, 0.0f, zCell * cellSize);
    }
    //turns grid indices into approximate world coordinates
    public Vector3 TupleToWorld(Vector2Int pair)
    {
        return new(pair.c * cellSize, 0.0f, pair.r * cellSize);
    }
}