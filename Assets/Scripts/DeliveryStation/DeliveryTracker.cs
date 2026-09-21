using System.Collections.Generic;
using UnityEngine;

public class DeliveryTracker : MonoBehaviour
{
    private Dictionary<FoodCategory, int> deliveredByCategory =
        new Dictionary<FoodCategory, int>();

    public int GetDeliveredCount(FoodCategory category)
    {
        if (deliveredByCategory.TryGetValue(category, out int count))
            return count;

        return 0;
    }

    public void RegisterDelivery(FoodCategory category, int quantity = 1)
    {
        if (quantity <= 0)
            return;

        if (!deliveredByCategory.ContainsKey(category))
            deliveredByCategory[category] = 0;

        deliveredByCategory[category] += quantity;

        Debug.Log(
            $"DeliveryTracker: delivered {quantity} item(s) of category " +
            $"{category}. Total: {deliveredByCategory[category]}."
        );
    }
}
