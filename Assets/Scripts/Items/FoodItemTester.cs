using UnityEngine;

public class FoodItemTester : MonoBehaviour
{
    [SerializeField] private GameObject foodItemPrefab;
    [SerializeField] private FoodItemData foodItemData;

    private void Start()
    {
        GameObject foodObject = Instantiate(
            foodItemPrefab,
            Vector3.zero,
            Quaternion.identity
        );

        FoodItem foodItem = foodObject.GetComponent<FoodItem>();

        foodItem.Initialize(foodItemData);

        Debug.Log(
            $"Spawned food: {foodItem.ItemData.ItemName}"
        );
    }
}
