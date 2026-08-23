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
            TryStartFromSpawner();
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

        if (nextGridObject == null)
        {
            return;
        }

        if (nextGridObject is ConveyorBelt)
        {
            if (IsCellOccupiedByFoodItem(nextPosition))
            {
                return;
            }

            targetGridPosition = nextPosition;
            isMoving = true;
        }
        else if (nextGridObject is Machine machine)
        {
            TryEnterMachine(machine);
        }
    }

    private void TryEnterMachine(Machine machine)
    {
        FoodItem foodItem =
            GetComponent<FoodItem>();

        Debug.Log(
            $"{foodItem.ItemData.ItemName} attempting to enter " +
            $"{machine.name}"
        );

        if (!machine.CanProcess(foodItem))
        {
            Debug.Log(
                $"{foodItem.ItemData.ItemName} cannot be processed by " +
                $"{machine.name} and is waiting."
            );

            return;
        }

        machine.Process(foodItem);
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

    private bool TryStartFromSpawner()
    {
        GridPosition[] directions =
        {
        new GridPosition(1, 0),   // Right
        new GridPosition(-1, 0),  // Left
        new GridPosition(0, 1),   // Up
        new GridPosition(0, -1)   // Down
    };

        ConveyorBelt foundBelt = null;
        GridPosition foundPosition = default;

        foreach (GridPosition direction in directions)
        {
            GridPosition checkPosition = new GridPosition(
                currentGridPosition.x + direction.x,
                currentGridPosition.y + direction.y
            );

            GridObject gridObject =
                gridManager.GetGridObject(checkPosition);

            if (gridObject is ConveyorBelt conveyorBelt)
            {
                if (foundBelt != null)
                {
                    Debug.LogWarning(
                        $"{name}: Multiple conveyor belts found next to spawner. " +
                        "Spawner output direction is ambiguous."
                    );

                    return false;
                }

                foundBelt = conveyorBelt;
                foundPosition = checkPosition;
            }
        }

        if (foundBelt == null)
        {
            return false;
        }

        if (IsCellOccupiedByFoodItem(foundPosition))
        {
            return false;
        }

        targetGridPosition = foundPosition;
        isMoving = true;

        return true;
    }

    private bool IsCellOccupiedByFoodItem(GridPosition position)
    {
        FoodItem[] foodItems =
            FindObjectsByType<FoodItem>(FindObjectsSortMode.None);

        foreach (FoodItem foodItem in foodItems)
        {
            if (foodItem == gameObject.GetComponent<FoodItem>())
                continue;

            GridPosition foodPosition =
                gridManager.WorldToGridPosition(
                    foodItem.transform.position
                );

            if (foodPosition.x == position.x &&
                foodPosition.y == position.y)
            {
                return true;
            }
        }

        return false;
    }
}