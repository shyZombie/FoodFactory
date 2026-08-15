using UnityEngine;

public class GridObject : MonoBehaviour
{
    private GridPosition gridPosition;

    public GridPosition GridPosition => gridPosition;

    public void SetGridPosition(GridPosition position)
    {
        gridPosition = position;
    }
}