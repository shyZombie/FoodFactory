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

        if (Recipe == null)
            return false;

        if (Recipe.Inputs == null ||
            Recipe.Inputs.Length == 0)
            return false;

        Recipe.Ingredient ingredient =
            Recipe.Inputs[0];

        if (ingredient.foodItem == null)
            return false;

        return foodItem.ItemData ==
               ingredient.foodItem;
    }

    public virtual void Process(FoodItem foodItem)
    {
        if (!CanProcess(foodItem))
            return;

        currentFoodItem = foodItem;
        processingTimer = 0f;
        isProcessing = true;

        Debug.Log(
            $"Machine started processing " +
            $"{foodItem.ItemData.ItemName}"
        );

        currentFoodItem.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (!isProcessing)
            return;

        if (Recipe == null)
            return;

        processingTimer += Time.deltaTime;

        if (processingTimer >= Recipe.ProcessingTime)
        {
            FinishProcessing();
        }
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

        CreateOutput();

        isProcessing = false;
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

        Recipe.Result result =
            Recipe.Outputs[0];

        FoodItemData outputItem =
            result.foodItem;

        if (outputItem == null)
        {
            Debug.LogError(
                $"{name} ERROR: Output FoodItemData is NULL!"
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

        outputFoodItem.Initialize(outputItem);

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
            return;
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

        Destroy(currentFoodItem.gameObject);

        currentFoodItem = null;
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
