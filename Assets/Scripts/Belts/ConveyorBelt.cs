using UnityEngine;

public class ConveyorBelt : MonoBehaviour
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

    public void SetDirection(Direction newDirection)
    {
        direction = newDirection;
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
