using UnityEngine;

public class FoodItemMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 2f;

    private GridManager gridManager;
    private ConveyorBelt currentBelt;

    private GridPosition currentGridPosition;
    private GridPosition targetGridPosition;

    private bool isMoving = false;

    public void Initialize(GridManager manager)
    {
        gridManager = manager;

        currentGridPosition =
            gridManager.WorldToGridPosition(transform.position);

        FindCurrentBelt();
    }

    private void Update()
    {
        if (gridManager == null)
            return;

        if (!isMoving)
        {
            StartMovingToNextCell();
        }

        MoveTowardsTarget();
    }

    private void FindCurrentBelt()
    {
        GridObject gridObject =
            gridManager.GetGridObject(currentGridPosition);

        if (gridObject is ConveyorBelt conveyorBelt)
        {
            currentBelt = conveyorBelt;
        }
        else
        {
            currentBelt = null;
        }
    }

    private void StartMovingToNextCell()
    {
        FindCurrentBelt();

        if (currentBelt == null)
        {
            return;
        }

        Vector2 direction =
            currentBelt.GetDirectionVector();

        GridPosition nextPosition = new GridPosition(
            currentGridPosition.x +
                Mathf.RoundToInt(direction.x),

            currentGridPosition.y +
                Mathf.RoundToInt(direction.y)
        );

        GridObject nextGridObject =
            gridManager.GetGridObject(nextPosition);

        // There is nothing in the next cell yet.
        // Wait until something is placed there.
        if (nextGridObject == null)
        {
            return;
        }

        if (nextGridObject is ConveyorBelt)
        {
            targetGridPosition = nextPosition;
            isMoving = true;
        }
    }

    private void MoveTowardsTarget()
    {
        if (!isMoving)
            return;

        Vector3 targetWorldPosition =
            gridManager.GridToWorldPosition(
                targetGridPosition
            );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWorldPosition,
            movementSpeed * Time.deltaTime
        );

        if (Vector3.Distance(
                transform.position,
                targetWorldPosition) < 0.001f)
        {
            transform.position = targetWorldPosition;

            currentGridPosition =
                targetGridPosition;

            isMoving = false;

            FindCurrentBelt();
        }
    }
}