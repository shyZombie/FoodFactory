using System.Collections.Generic;
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
    [SerializeField]
    protected GameObject foodItemPrefab;
    protected List<FoodItem> storedIngredients =
    new List<FoodItem>();
    protected Queue<FoodItemData> pendingOutputs =
        new Queue<FoodItemData>();


    public Recipe Recipe => recipe;

    [SerializeField]
    private Direction direction =
        Direction.Right;

    protected GridManager gridManager;

    protected FoodItem currentFoodItem;
    protected float processingTimer;
    protected bool isProcessing;



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
        if (foodItem == null)
            return false;

        if (isProcessing)
            return false;

        if (Recipe == null)
            return false;

        return Recipe.HasIngredient(
            foodItem.ItemData,
            storedIngredients
        );
    }

    public virtual bool TryAcceptIngredient(
    FoodItem foodItem)
    {
        if (!CanProcess(foodItem))
            return false;

        if (storedIngredients.Contains(foodItem))
            return false;

        storedIngredients.Add(foodItem);

        foodItem.gameObject.SetActive(false);

        Debug.Log(
            $"{name} accepted ingredient: " +
            $"{foodItem.ItemData.ItemName}" +
            $" | Stored ingredients: {storedIngredients.Count}"
        );

        return true;
    }

    public virtual void Process(FoodItem foodItem)
    {
        if (!TryAcceptIngredient(foodItem))
            return;

        if (!Recipe.HasEnoughIngredients(
            storedIngredients))
        {
            Debug.Log(
                $"{name} is waiting for more ingredients."
            );

            return;
        }

        StartProcessing();
    }

    protected virtual void StartProcessing()
    {
        if (storedIngredients.Count == 0)
            return;

        currentFoodItem =
            storedIngredients[0];

        processingTimer = 0f;
        isProcessing = true;

        Debug.Log(
            $"{name} started processing recipe."
        );
    }

    protected virtual void ConsumeIngredients()
    {
        foreach (FoodItem ingredient
                 in storedIngredients)
        {
            if (ingredient != null)
            {
                Destroy(ingredient.gameObject);
            }
        }

        storedIngredients.Clear();

        currentFoodItem = null;
    }

    protected virtual void Update()
    {
        if (isProcessing)
        {
            if (Recipe == null)
                return;

            processingTimer += Time.deltaTime;

            if (processingTimer >= Recipe.ProcessingTime)
            {
                FinishProcessing();
            }

            return;
        }

        TryCreateNextOutput();
    }

    protected virtual void FinishProcessing()
    {
        if (currentFoodItem == null)
        {
            isProcessing = false;
            return;
        }

        Debug.Log(
            $"Machine finished processing " +
            $"{currentFoodItem.ItemData.ItemName}"
        );

        QueueRecipeOutputs();

        ConsumeIngredients();

        isProcessing = false;

        TryCreateNextOutput();
    }

    protected virtual void QueueRecipeOutputs()
    {
        if (Recipe == null)
            return;

        if (Recipe.Outputs == null)
            return;

        foreach (Recipe.Result result in Recipe.Outputs)
        {
            if (result == null)
                continue;

            if (result.foodItem == null)
            {
                Debug.LogError(
                    $"{name} ERROR: Output FoodItemData is NULL!"
                );

                continue;
            }

            for (int i = 0;
                 i < result.quantity;
                 i++)
            {
                pendingOutputs.Enqueue(
                    result.foodItem
                );
            }
        }
    }

    protected virtual void TryCreateNextOutput()
    {
        if (pendingOutputs.Count == 0)
            return;

        if (gridManager == null)
            return;

        if (foodItemPrefab == null)
            return;

        Vector2 outputDirection =
            GetOutputDirectionVector();

        GridPosition machineGridPosition =
            GridPosition;

        GridPosition outputGridPosition =
            new GridPosition(
                machineGridPosition.x +
                    Mathf.RoundToInt(outputDirection.x),

                machineGridPosition.y +
                    Mathf.RoundToInt(outputDirection.y)
            );

        GridObject outputGridObject =
            gridManager.GetGridObject(
                outputGridPosition
            );

        if (outputGridObject != null &&
            outputGridObject is not ConveyorBelt)
        {
            Debug.Log(
                $"{name} output blocked at " +
                $"{outputGridPosition}"
            );

            return;
        }

        if (IsOutputPositionOccupied(
            outputGridPosition))
        {
            Debug.Log(
                $"{name} output blocked by FoodItem at " +
                $"{outputGridPosition}"
            );

            return;
        }

        FoodItemData outputItem =
            pendingOutputs.Dequeue();

        Vector3 outputPosition =
            gridManager.GridToWorldPosition(
                outputGridPosition
            );

        GameObject outputObject =
            Instantiate(
                foodItemPrefab,
                outputPosition,
                Quaternion.identity
            );

        FoodItem outputFoodItem =
            outputObject.GetComponent<FoodItem>();

        if (outputFoodItem == null)
        {
            Debug.LogError(
                $"{name} ERROR: Food Item Prefab " +
                "does not contain FoodItem!"
            );

            Destroy(outputObject);

            return;
        }

        outputFoodItem.Initialize(
            outputItem
        );

        FoodItemMovement movement =
            outputObject.GetComponent<FoodItemMovement>();

        if (movement == null)
        {
            Debug.LogError(
                $"{name} ERROR: Food Item Prefab " +
                "does not contain FoodItemMovement!"
            );

            Destroy(outputObject);

            return;
        }

        movement.Initialize(gridManager);

        Debug.Log(
            $"{name} created output: " +
            $"{outputFoodItem.ItemData.ItemName}"
        );
    }

    protected virtual void CreateOutput()
    {
        if (Recipe == null)
        {
            Debug.LogError(
                $"{name} ERROR: Recipe is NULL!"
            );

            return;
        }

        if (Recipe.Outputs == null ||
            Recipe.Outputs.Length == 0)
        {
            Debug.LogError(
                $"{name} ERROR: Recipe has no outputs!"
            );

            return;
        }

        if (gridManager == null)
        {
            Debug.LogError(
                $"{name} ERROR: GridManager is NULL!"
            );

            return;
        }

        if (currentFoodItem == null)
        {
            Debug.LogError(
                $"{name} ERROR: Current Food Item is NULL!"
            );

            return;
        }

        Vector2 outputDirection =
            GetOutputDirectionVector();

        GridPosition machineGridPosition =
            GridPosition;

        GridPosition outputGridPosition =
            new GridPosition(
                machineGridPosition.x +
                    Mathf.RoundToInt(outputDirection.x),

                machineGridPosition.y +
                    Mathf.RoundToInt(outputDirection.y)
            );

        GridObject outputGridObject =
            gridManager.GetGridObject(
                outputGridPosition
            );

        if (IsOutputPositionOccupied(outputGridPosition))
        {
            Debug.Log(
                $"{name} output blocked by FoodItem at " +
                $"{outputGridPosition}"
            );

            return;
        }

        if (outputGridObject != null &&
            outputGridObject is not ConveyorBelt)
        {
            Debug.Log(
                $"{name} output blocked at " +
                $"{outputGridPosition}"
            );

            return;
        }

        Vector3 outputPosition =
            gridManager.GridToWorldPosition(
                outputGridPosition
            );

        foreach (Recipe.Result result in Recipe.Outputs)
        {
            if (result == null)
                continue;

            if (result.foodItem == null)
            {
                Debug.LogError(
                    $"{name} ERROR: Output FoodItemData is NULL!"
                );

                continue;
            }

            for (int i = 0;
                 i < result.quantity;
                 i++)
            {
                GameObject outputObject =
                    Instantiate(
                        foodItemPrefab,
                        outputPosition,
                        Quaternion.identity
                    );

                FoodItem outputFoodItem =
                    outputObject.GetComponent<FoodItem>();

                if (outputFoodItem == null)
                {
                    Debug.LogError(
                        $"{name} ERROR: Food Item Prefab " +
                        "does not contain FoodItem!"
                    );

                    Destroy(outputObject);
                    continue;
                }

                outputFoodItem.Initialize(
                    result.foodItem
                );

                Debug.Log(
                    $"{name} created output: " +
                    $"{outputFoodItem.ItemData.ItemName}"
                );

                FoodItemMovement movement =
                    outputObject.GetComponent<FoodItemMovement>();

                if (movement == null)
                {
                    Debug.LogError(
                        $"{name} ERROR: Food Item Prefab " +
                        "does not contain FoodItemMovement!"
                    );

                    Destroy(outputObject);
                    continue;
                }

                movement.Initialize(gridManager);

                if (outputGridObject is ConveyorBelt)
                {
                    Debug.Log(
                        $"{name} output connected to conveyor at " +
                        $"{outputGridPosition}"
                    );
                }
                else
                {
                    Debug.Log(
                        $"{name} output waiting at " +
                        $"{outputGridPosition}"
                    );
                }
            }
        }

        ConsumeIngredients();
    }

    private bool IsOutputPositionOccupied(
    GridPosition position
)
    {
        FoodItem[] foodItems =
            FindObjectsByType<FoodItem>(
                FindObjectsSortMode.None
            );

        foreach (FoodItem foodItem in foodItems)
        {
            if (foodItem == null)
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
        if (Recipe == null)
        {
            Debug.LogError(
                $"{name} ERROR: Recipe is NULL!"
            );

            return;
        }

        if (!Recipe.IsValid())
        {
            Debug.LogError(
                $"{name} ERROR: Assigned Recipe is invalid. " +
                $"Reason: {Recipe.GetValidationError()}"
            );

            return;
        }

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
