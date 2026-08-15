using UnityEngine;

public class FoodMovementTester : MonoBehaviour
{
    [SerializeField] private GameObject foodItemPrefab;
    [SerializeField] private FoodItemData foodItemData;
    [SerializeField] private GridManager gridManager;

    private void Start()
    {
        Vector3 spawnPosition =
            gridManager.GridToWorldPosition(
                new GridPosition(0, 0)
            );

        GameObject foodObject = Instantiate(
            foodItemPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FoodItem foodItem =
            foodObject.GetComponent<FoodItem>();

        foodItem.Initialize(foodItemData);

        FoodItemMovement movement =
            foodObject.GetComponent<FoodItemMovement>();

        movement.Initialize(gridManager);
    }
}