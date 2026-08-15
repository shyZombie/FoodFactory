using UnityEngine;

public class Cutter : Machine
{
    [SerializeField] private GameObject foodItemPrefab;
    //[SerializeField] private Vector2 outputDirection = Vector2.right;

    private FoodItem currentFoodItem;
    private float processingTimer;
    private bool isProcessing;

    public override bool CanProcess(FoodItem foodItem)
    {
        if (isProcessing)
            return false;

        if (Recipe == null)
            return false;

        if (Recipe.Inputs == null ||
            Recipe.Inputs.Length == 0)
            return false;

        Recipe.Ingredient ingredient =
            Recipe.Inputs[0];

        return foodItem.ItemData ==
               ingredient.foodItem;
    }

    public override void Process(FoodItem foodItem)
    {
        if (!CanProcess(foodItem))
            return;

        currentFoodItem = foodItem;
        processingTimer = 0f;
        isProcessing = true;

        Debug.Log(
            $"Cutter started processing " +
            $"{foodItem.ItemData.ItemName}"
        );

        currentFoodItem.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isProcessing)
            return;

        processingTimer += Time.deltaTime;

        if (processingTimer >= Recipe.ProcessingTime)
        {
            FinishProcessing();
        }
    }

    private void FinishProcessing()
    {
        isProcessing = false;

        if (Recipe == null)
        {
            Debug.LogError(
                "Cutter ERROR: Recipe is NULL!"
            );

            return;
        }

        if (Recipe.Outputs == null ||
            Recipe.Outputs.Length == 0)
        {
            Debug.LogError(
                "Cutter ERROR: Recipe has no outputs!"
            );

            return;
        }

        Recipe.Result result =
            Recipe.Outputs[0];

        FoodItemData outputItem =
            result.foodItem;

        Debug.Log("Cutter: FinishProcessing started.");

        if (outputItem == null)
        {
            Debug.LogError(
                "Cutter ERROR: Output Item is NULL!"
            );

            return;
        }

        if (foodItemPrefab == null)
        {
            Debug.LogError(
                "Cutter ERROR: Food Item Prefab is NULL!"
            );

            return;
        }

        if (gridManager == null)
        {
            Debug.LogError(
                "Cutter ERROR: GridManager is NULL!"
            );

            return;
        }

        if (currentFoodItem == null)
        {
            Debug.LogError(
                "Cutter ERROR: Current Food Item is NULL!"
            );

            return;
        }

        Vector2 outputDirection =
            GetOutputDirectionVector();

        Debug.Log(
            $"Cutter output direction: {GetDirection()}"
        );

        Debug.Log(
            $"Cutter output vector: {outputDirection}"
        );

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
                $"Cutter output blocked at " +
                $"{outputGridPosition}"
            );

            isProcessing = false;

            return;
        }

        Vector3 outputPosition =
            gridManager.GridToWorldPosition(
                outputGridPosition
            );

        GameObject outputObject = Instantiate(
            foodItemPrefab,
            outputPosition,
            Quaternion.identity
        );

        FoodItem outputFoodItem =
            outputObject.GetComponent<FoodItem>();

        if (outputFoodItem == null)
        {
            Debug.LogError(
                "Cutter ERROR: Food Item Prefab " +
                "does not contain FoodItem!"
            );

            Destroy(outputObject);
            return;
        }

        outputFoodItem.Initialize(outputItem);

        FoodItemMovement movement =
            outputObject.GetComponent<FoodItemMovement>();

        if (movement == null)
        {
            Debug.LogError(
                "Cutter ERROR: Food Item Prefab " +
                "does not contain FoodItemMovement!"
            );

            Destroy(outputObject);
            return;
        }

        movement.Initialize(gridManager);

        if (outputGridObject is ConveyorBelt)
        {
            Debug.Log(
                $"Cutter output connected to conveyor at " +
                $"{outputGridPosition}"
            );
        }
        else
        {
            Debug.Log(
                $"Cutter output waiting at " +
                $"{outputGridPosition}"
            );
        }

        Debug.Log(
            $"Cutter finished: {currentFoodItem.ItemData.ItemName} → " +
            $"{outputItem.ItemName}"
        );

        Destroy(currentFoodItem.gameObject);

        currentFoodItem = null;
    }
}