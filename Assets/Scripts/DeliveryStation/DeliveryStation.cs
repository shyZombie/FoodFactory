using Unity.VisualScripting;
using UnityEngine;

public class DeliveryStation : GridObject
{
    [SerializeField] private DeliveryTracker deliveryTracker;
    public void ReceiveFood(FoodItem foodItem)
    {
        if (foodItem == null)
            return;

        Debug.Log(
            $"{name} received {foodItem.ItemData.ItemName}."
        );

        foreach (FoodCategory category in
                 System.Enum.GetValues(typeof(FoodCategory)))
        {
            if (foodItem.ItemData.HasCategory(category))
            {
                deliveryTracker.RegisterDelivery(category);
            }
        }

        Destroy(foodItem.gameObject);
    }
}
