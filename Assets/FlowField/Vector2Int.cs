
using System;

public struct Vector2Int
{
    public int c;
    public int r;
    public int hashCode;

    public Vector2Int(int c, int r, int row)
    {
        this.c = c;
        this.r = r;
        hashCode = c * row + r;
    }

    public override readonly int GetHashCode()
    {
        return hashCode;
    }
}