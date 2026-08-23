using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodItemPrefab;
    [SerializeField] private FoodItemData foodItemData;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Extractor extractor;

    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxItems = 5;
    private GameObject currentFoodObject;

    private List<GameObject> spawnedFoodObjects =
        new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(
            nameof(TrySpawnFood),
            0f,
            spawnInterval
        );
    }

    private void TrySpawnFood()
    {
        if (extractor == null)
        {
            return;
        }

        GridPosition spawnPosition;

        if (!extractor.TryGetAvailableExtractionSlot(
            out spawnPosition))
        {
            return;
        }

        RemoveInactiveFoodObjects();

        if (spawnedFoodObjects.Count >= maxItems)
        {
            return;
        }

        if (IsSpawnCellOccupied(spawnPosition))
        {
            return;
        }

        SpawnFood(spawnPosition);
    }

    private void RemoveInactiveFoodObjects()
    {
        spawnedFoodObjects.RemoveAll(
            foodObject =>
                foodObject == null ||
                !foodObject.activeSelf
        );
    }

    private bool IsSpawnCellOccupied(
        GridPosition spawnGridPosition
    )
    {
        Vector3 spawnPosition =
            gridManager.GridToWorldPosition(
                spawnGridPosition
            );

        FoodItem[] foodItems =
            FindObjectsByType<FoodItem>(
                FindObjectsSortMode.None
            );

        foreach (FoodItem foodItem in foodItems)
        {
            if (Vector3.Distance(
                    foodItem.transform.position,
                    spawnPosition
                ) < 0.1f)
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnFood(
        GridPosition spawnGridPosition
    )
    {
        Vector3 spawnWorldPosition =
            gridManager.GridToWorldPosition(
                spawnGridPosition
            );

        currentFoodObject = Instantiate(
            foodItemPrefab,
            spawnWorldPosition,
            Quaternion.identity
        );

        FoodItem foodItem =
            currentFoodObject.GetComponent<FoodItem>();

        foodItem.Initialize(foodItemData);

        FoodItemMovement movement =
            currentFoodObject.GetComponent<FoodItemMovement>();

        movement.Initialize(gridManager);
    }
}