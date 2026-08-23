using System.Collections.Generic;
using UnityEngine;

public class Extractor : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private FoodSpawner foodSpawner;

    private GridPosition extractorCenterPosition;

    private List<GridPosition> extractionSlots =
        new List<GridPosition>();

    private void Awake()
    {
        if (gridManager == null)
        {
            Debug.LogError(
                "Extractor ERROR: GridManager is NULL!"
            );

            return;
        }

        extractorCenterPosition =
            gridManager.WorldToGridPosition(
                transform.position
            );

        GenerateExtractionSlots();
    }

    private void Start()
    {
        List<GridPosition> activeSlots =
            GetActiveExtractionSlots();

        foreach (GridPosition slot in activeSlots)
        {
            Debug.Log(
                $"Active extraction slot: " +
                $"({slot.x}, {slot.y})"
            );
        }

        TestAvailableSlot();
    }

    private void GenerateExtractionSlots()
    {
        extractionSlots.Clear();

        int minX = extractorCenterPosition.x - 1;
        int maxX = extractorCenterPosition.x + 1;

        int minY = extractorCenterPosition.y - 1;
        int maxY = extractorCenterPosition.y + 1;

        // Right
        for (int y = minY; y <= maxY; y++)
        {
            extractionSlots.Add(
                new GridPosition(
                    maxX + 1,
                    y
                )
            );
        }

        // Left
        for (int y = minY; y <= maxY; y++)
        {
            extractionSlots.Add(
                new GridPosition(
                    minX - 1,
                    y
                )
            );
        }

        // Up
        for (int x = minX; x <= maxX; x++)
        {
            extractionSlots.Add(
                new GridPosition(
                    x,
                    maxY + 1
                )
            );
        }

        // Down
        for (int x = minX; x <= maxX; x++)
        {
            extractionSlots.Add(
                new GridPosition(
                    x,
                    minY - 1
                )
            );
        }

        //Temporary for testing
        foreach (GridPosition slot in extractionSlots)
        {
            Debug.Log(
                $"Extractor slot: ({slot.x}, {slot.y})"
            );
        }
    }

    private List<GridPosition> GetActiveExtractionSlots()
    {
        List<GridPosition> activeSlots =
            new List<GridPosition>();

        foreach (GridPosition slot in extractionSlots)
        {
            GridObject gridObject =
                gridManager.GetGridObject(slot);

            Debug.Log(
                $"Checking extractor slot ({slot.x}, {slot.y}) → " +
                $"{(gridObject == null ? "EMPTY" : gridObject.GetType().Name)}"
            );

            if (gridObject is ConveyorBelt)
            {
                activeSlots.Add(slot);
            }
        }

        return activeSlots;
    }

    public bool TryGetAvailableExtractionSlot(
    out GridPosition availableSlot
)
    {
        List<GridPosition> activeSlots =
            GetActiveExtractionSlots();

        foreach (GridPosition slot in activeSlots)
        {
            if (!IsExtractionSlotOccupied(slot))
            {
                availableSlot = slot;
                return true;
            }
        }

        availableSlot = default;
        return false;
    }

    private bool IsExtractionSlotOccupied(
    GridPosition slot
)
    {
        FoodItem[] foodItems =
            FindObjectsByType<FoodItem>(
                FindObjectsSortMode.None
            );

        foreach (FoodItem foodItem in foodItems)
        {
            GridPosition foodPosition =
                gridManager.WorldToGridPosition(
                    foodItem.transform.position
                );

            if (foodPosition.x == slot.x &&
                foodPosition.y == slot.y)
            {
                return true;
            }
        }

        return false;
    }

    //Temporary for testing
    private void TestAvailableSlot()
    {
        if (TryGetAvailableExtractionSlot(
            out GridPosition slot))
        {
            Debug.Log(
                $"Extractor available slot: " +
                $"({slot.x}, {slot.y})"
            );
        }
        else
        {
            Debug.Log(
                "Extractor has no available extraction slot."
            );
        }
    }

    public List<GridPosition> GetActiveSlots()
    {
        return GetActiveExtractionSlots();
    }
    public bool HasActiveExtractionSlots()
    {
        return GetActiveExtractionSlots().Count > 0;
    }

    public List<GridPosition> GetExtractionSlots()
    {
        return extractionSlots;
    }

    public GridPosition GetExtractorCenterPosition()
    {
        return extractorCenterPosition;
    }

    public FoodSpawner GetFoodSpawner()
    {
        return foodSpawner;
    }
}