using System.Collections.Generic;
using UnityEngine;

public class Extractor : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private FoodSpawner foodSpawner;

    private GridPosition extractorGridPosition;

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

        extractorGridPosition =
            gridManager.WorldToGridPosition(
                transform.position
            );

        GenerateExtractionSlots();
    }

    private void GenerateExtractionSlots()
    {
        extractionSlots.Clear();

        // Right
        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 1,
                extractorGridPosition.y
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 1,
                extractorGridPosition.y + 1
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 1,
                extractorGridPosition.y + 2
            )
        );

        // Left
        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x - 1,
                extractorGridPosition.y
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x - 1,
                extractorGridPosition.y + 1
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x - 1,
                extractorGridPosition.y + 2
            )
        );

        // Up
        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x,
                extractorGridPosition.y + 1
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 1,
                extractorGridPosition.y + 1
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 2,
                extractorGridPosition.y + 1
            )
        );

        // Down
        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x,
                extractorGridPosition.y - 1
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 1,
                extractorGridPosition.y - 1
            )
        );

        extractionSlots.Add(
            new GridPosition(
                extractorGridPosition.x + 2,
                extractorGridPosition.y - 1
            )
        );

        foreach (GridPosition slot in extractionSlots)
        {
            Debug.Log(
                $"Extractor slot: ({slot.x}, {slot.y})"
            );
        }
    }

    public List<GridPosition> GetExtractionSlots()
    {
        return extractionSlots;
    }

    public GridPosition GetExtractorGridPosition()
    {
        return extractorGridPosition;
    }

    public FoodSpawner GetFoodSpawner()
    {
        return foodSpawner;
    }
}
