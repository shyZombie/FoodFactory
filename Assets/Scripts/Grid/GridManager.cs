using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private float cellSize = 1f;

    private HashSet<GridPosition> occupiedCells = new HashSet<GridPosition>();

    public GridPosition WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);

        return new GridPosition(x, y);
    }

    public Vector3 GridToWorldPosition(GridPosition gridPosition)
    {
        float x = gridPosition.x * cellSize + cellSize / 2f;
        float y = gridPosition.y * cellSize + cellSize / 2f;

        return new Vector3(x, y, 0f);
    }

    public bool IsCellOccupied(GridPosition gridPosition)
    {
        return occupiedCells.Contains(gridPosition);
    }

    public bool TryOccupyCell(GridPosition gridPosition)
    {
        if (IsCellOccupied(gridPosition))
        {
            return false;
        }

        occupiedCells.Add(gridPosition);
        return true;
    }

    public void FreeCell(GridPosition gridPosition)
    {
        occupiedCells.Remove(gridPosition);
    }
}