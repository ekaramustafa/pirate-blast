using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPosition
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"x : {x}, y : {y}";
    }

    public override bool Equals(object obj)
    {
        if (!(obj is GridPosition))
            return false;

        GridPosition other = (GridPosition)obj;
        return this.x == other.x && this.y == other.y;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode();
    }

    public static bool operator ==(GridPosition pos1, GridPosition pos2)
    {
        return pos1.Equals(pos2);
    }

    public static bool operator !=(GridPosition pos1, GridPosition pos2)
    {
        return !pos1.Equals(pos2);
    }

}
