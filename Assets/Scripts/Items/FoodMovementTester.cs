using UnityEngine;

public class FoodMovementTester : MonoBehaviour
{
    [SerializeField] private GameObject foodItemPrefab;
    [SerializeField] private FoodItemData foodItemData;
    [SerializeField] private GridManager gridManager;
    [SerializeField]
    private UpgradeManager upgradeManager;

    [SerializeField] private int spawnGridX = 0;
    [SerializeField] private int spawnGridY = 0;

    private void Start()
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

        FoodItem foodItem =
            foodObject.GetComponent<FoodItem>();

        foodItem.Initialize(foodItemData);

        FoodItemMovement movement =
            foodObject.GetComponent<FoodItemMovement>();

        movement.Initialize(
            gridManager,
            upgradeManager
        );
    }
}