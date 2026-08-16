using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [SerializeField] private GameObject foodItemPrefab;
    [SerializeField] private FoodItemData foodItemData;
    [SerializeField] private GridManager gridManager;

    [SerializeField] private int spawnGridX = 0;
    [SerializeField] private int spawnGridY = 0;

    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxItems = 5;

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
        RemoveInactiveFoodObjects();

        if (spawnedFoodObjects.Count >= maxItems)
        {
            return;
        }

        if (IsSpawnCellOccupied())
        {
            return;
        }

        SpawnFood();
    }

    private void RemoveInactiveFoodObjects()
    {
        spawnedFoodObjects.RemoveAll(
            foodObject =>
                foodObject == null ||
                !foodObject.activeSelf
        );
    }

    private bool IsSpawnCellOccupied()
    {
        Vector3 spawnPosition =
            gridManager.GridToWorldPosition(
                new GridPosition(
                    spawnGridX,
                    spawnGridY
                )
            );

        foreach (GameObject foodObject in spawnedFoodObjects)
        {
            if (foodObject == null ||
                !foodObject.activeSelf)
            {
                continue;
            }

            if (Vector3.Distance(
                    foodObject.transform.position,
                    spawnPosition) < 0.01f)
            {
                return true;
            }
        }

        return false;
    }

    private void SpawnFood()
    {
        Vector3 spawnPosition =
            gridManager.GridToWorldPosition(
                new GridPosition(
                    spawnGridX,
                    spawnGridY
                )
            );

        GameObject foodObject = Instantiate(
            foodItemPrefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedFoodObjects.Add(foodObject);

        FoodItem foodItem =
            foodObject.GetComponent<FoodItem>();

        foodItem.Initialize(foodItemData);

        FoodItemMovement movement =
            foodObject.GetComponent<FoodItemMovement>();

        movement.Initialize(gridManager);
    }
}