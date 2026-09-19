using UnityEngine;

public class DeliveryStation : GridObject
{
    public void ReceiveFood(FoodItem foodItem)
    {
        if (foodItem == null)
            return;

        Debug.Log(
            $"{name} received {foodItem.ItemData.ItemName}."
        );

        Destroy(foodItem.gameObject);
    }
}
