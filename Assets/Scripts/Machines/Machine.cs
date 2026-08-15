using UnityEngine;

public class Machine : GridObject
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    //[SerializeField] protected float processingTime = 1f;
    [SerializeField] protected Recipe recipe;

    public Recipe Recipe => recipe;

    [SerializeField]
    private Direction direction =
        Direction.Right;

    protected GridManager gridManager;

    public Direction GetDirection()
    {
        return direction;
    }

    public Vector2 GetOutputDirectionVector()
    {
        return direction switch
        {
            Direction.Up => Vector2.up,
            Direction.Right => Vector2.right,
            Direction.Down => Vector2.down,
            Direction.Left => Vector2.left,
            _ => Vector2.right
        };
    }

    public Vector2 GetInputDirectionVector()
    {
        return -GetOutputDirectionVector();
    }

    public void Initialize(GridManager manager)
    {
        gridManager = manager;
    }

    public virtual bool CanProcess(FoodItem foodItem)
    {
        return true;
    }

    public virtual void Process(FoodItem foodItem)
    {
        Debug.Log(
            $"Machine processing {foodItem.ItemData.ItemName}"
        );
    }

    public virtual void RotateClockwise()
    {
        direction = direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => Direction.Right
        };

        Debug.Log($"Machine rotated. New direction: {direction}");

        UpdateVisualRotation();
    }

    protected virtual void Start()
    {
        UpdateVisualRotation();
    }

    protected virtual void UpdateVisualRotation()
    {
        float rotation = direction switch
        {
            Direction.Up => 90f,
            Direction.Right => 0f,
            Direction.Down => 270f,
            Direction.Left => 180f,
            _ => 0f
        };

        transform.rotation =
            Quaternion.Euler(0f, 0f, rotation);
    }
}
