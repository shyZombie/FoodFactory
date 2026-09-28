using System.Collections.Generic;
using UnityEngine;

public class Machine : GridObject
{
    public static event System.Action<Recipe, FoodItemData>
        OnFoodItemProduced;
    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    //[SerializeField] protected float processingTime = 1f;
    [SerializeField] protected Recipe[] recipes;
    protected Recipe currentRecipe;
    [SerializeField]
    protected GameObject foodItemPrefab;
    protected List<FoodItem> storedIngredients =
    new List<FoodItem>();
    protected Queue<FoodItemData> pendingOutputs =
        new Queue<FoodItemData>();
    protected Queue<Recipe> pendingOutputRecipes =
        new Queue<Recipe>();


    public Recipe Recipe => currentRecipe;

    [SerializeField]
    private Direction direction =
        Direction.Right;
    [SerializeField]
    private UpgradeManager upgradeManager;
    [SerializeField]
    private UpgradeTarget upgradeTarget;
    [SerializeField]
    private RecipeDiscoveryManager recipeDiscoveryManager;

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

        if (recipes == null || recipes.Length == 0)
            return false;

        foreach (Recipe candidateRecipe in recipes)
        {
            if (candidateRecipe == null ||
                candidateRecipe.Inputs == null)
            {
                continue;
            }

            foreach (Recipe.Ingredient ingredient
                     in candidateRecipe.Inputs)
            {
                if (ingredient == null)
                    continue;

                if (ingredient.foodItem ==
                    foodItem.ItemData)
                {
                    return true;
                }
            }
        }

        return false;
    }
    protected virtual List<Recipe> FindMatchingRecipes(FoodItem foodItem)
    {
        List<Recipe> matchingRecipes = new List<Recipe>();

        if (foodItem == null)
            return matchingRecipes;

        if (recipes == null || recipes.Length == 0)
            return matchingRecipes;

        foreach (Recipe candidateRecipe in recipes)
        {
            if (candidateRecipe == null)
                continue;

            if (candidateRecipe.HasIngredient(
                foodItem.ItemData,
                storedIngredients))
            {
                matchingRecipes.Add(candidateRecipe);
            }
        }

        return matchingRecipes;
    }

    public virtual bool TryAcceptIngredient(
    FoodItem foodItem)
    {
        if (isProcessing)
        {
            return false;
        }

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

        if (!TryResolveRecipe())
        {
            Debug.Log(
                $"{name} is waiting for more ingredients."
            );

            return;
        }

        StartProcessing();
    }

    protected virtual bool TryResolveRecipe()
    {
        if (recipes == null || recipes.Length == 0)
            return false;

        foreach (Recipe candidateRecipe in recipes)
        {
            if (candidateRecipe == null)
                continue;

            if (!candidateRecipe.HasEnoughIngredients(
                    storedIngredients))
            {
                continue;
            }

            currentRecipe = candidateRecipe;

            if (recipeDiscoveryManager != null &&
                !recipeDiscoveryManager.IsDiscovered(currentRecipe))
            {
                recipeDiscoveryManager.DiscoverRecipe(
                    currentRecipe
                );
            }

            Debug.Log(
                $"{name} resolved recipe: " +
                $"{currentRecipe.name}"
            );

            return true;
        }

        currentRecipe = null;
        return false;
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
        if (currentRecipe == null ||
            currentRecipe.Inputs == null)
        {
            return;
        }

        List<FoodItem> consumedIngredients =
            new List<FoodItem>();

        foreach (Recipe.Ingredient recipeIngredient
                 in currentRecipe.Inputs)
        {
            if (recipeIngredient == null ||
                recipeIngredient.foodItem == null)
            {
                continue;
            }

            int requiredQuantity =
                recipeIngredient.quantity;

            foreach (FoodItem storedItem in storedIngredients)
            {
                if (requiredQuantity <= 0)
                    break;

                if (storedItem == null ||
                    consumedIngredients.Contains(storedItem))
                {
                    continue;
                }

                if (storedItem.ItemData !=
                    recipeIngredient.foodItem)
                {
                    continue;
                }

                consumedIngredients.Add(storedItem);
                requiredQuantity--;
            }
        }

        foreach (FoodItem consumedItem in consumedIngredients)
        {
            if (consumedItem != null)
            {
                Destroy(consumedItem.gameObject);
            }
        }

        storedIngredients.RemoveAll(
            ingredient =>
                ingredient == null ||
                consumedIngredients.Contains(ingredient)
        );

        currentFoodItem = null;
    }

    protected virtual void Update()
    {
        if (isProcessing)
        {
            if (Recipe == null)
                return;

            processingTimer += Time.deltaTime;

            if (processingTimer >= GetEffectiveProcessingTime())
            {
                FinishProcessing();
            }

            return;
        }

        TryCreateNextOutput();
    }

    protected virtual float GetEffectiveProcessingTime()
    {
        if (Recipe == null)
        {
            return 0f;
        }

        float speedMultiplier = 1f;

        if (upgradeManager != null)
        {
            speedMultiplier =
                upgradeManager.GetSpeedMultiplier(
                    upgradeTarget
                );
        }

        if (speedMultiplier <= 0f)
        {
            speedMultiplier = 1f;
        }

        float effectiveTime = Recipe.ProcessingTime /
       speedMultiplier;
        Debug.Log(
            $"[{name}] Recipe: {Recipe.name} | " +
            $"Base Time: {Recipe.ProcessingTime:F2}s | " +
            $"Speed Multiplier: {speedMultiplier:F2} | " +
            $"Effective Time: {effectiveTime:F2}s"
        );

        return Recipe.ProcessingTime /
               speedMultiplier;
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

        currentRecipe = null;

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

                pendingOutputRecipes.Enqueue(
                    currentRecipe
                );
            }
        }
    }

    protected virtual void TryCreateNextOutput()
    {
        if (pendingOutputs.Count == 0)
            return;

        if (pendingOutputRecipes.Count == 0)
        {
            Debug.LogError(
                $"{name} ERROR: Pending output recipe queue is empty."
            );

            return;
        }

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

        if (outputGridObject is not ConveyorBelt)
        {
            Debug.Log(
                $"{name} output waiting for conveyor at " +
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
            pendingOutputs.Peek();

        Recipe outputRecipe =
            pendingOutputRecipes.Peek();

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

        OnFoodItemProduced?.Invoke(
            outputRecipe,
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

        movement.Initialize(
            gridManager,
            upgradeManager
        );

        pendingOutputs.Dequeue();
        pendingOutputRecipes.Dequeue();

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

                movement.Initialize(
                    gridManager,
                    upgradeManager
                );

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
