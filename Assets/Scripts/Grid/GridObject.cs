using UnityEngine;

public class GridObject : MonoBehaviour
{
    private GridPosition gridPosition;

    [SerializeField] private int width = 1;
    [SerializeField] private int height = 1;

    public GridPosition GridPosition => gridPosition;

    public int Width => width;
    public int Height => height;

    public void SetGridPosition(GridPosition position)
    {
        gridPosition = position;
    }
}