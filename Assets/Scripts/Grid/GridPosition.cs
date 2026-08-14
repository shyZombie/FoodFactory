using System;

[Serializable]
public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public bool Equals(GridPosition other)
    {
        return x == other.x && y == other.y;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public override string ToString()
    {
        return $"({x}, {y})";
    }
}