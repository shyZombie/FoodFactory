using UnityEngine;

public class ConveyorBelt : GridObject
{
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    [SerializeField] private Direction direction = Direction.Right;

    public Direction GetDirection()
    {
        return direction;
    }

    public Vector2 GetDirectionVector()
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

    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
        UpdateVisualRotation();
    }
    public void RotateClockwise()
    {
        direction = direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => Direction.Right
        };

        UpdateVisualRotation();
    }

    private void Start()
    {
        UpdateVisualRotation();
    }

    private void UpdateVisualRotation()
    {
        float rotation = direction switch
        {
            Direction.Up => 90f,
            Direction.Right => 0f,
            Direction.Down => 270f,
            Direction.Left => 180f,
            _ => 0f
        };

        transform.rotation = Quaternion.Euler(0f, 0f, rotation);
    }
}
