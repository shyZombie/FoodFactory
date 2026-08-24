using System.Collections.Generic;
using UnityEngine;

public class ProductionTracker : MonoBehaviour
{
    private Dictionary<FoodCategory, int> categoryCounts =
        new Dictionary<FoodCategory, int>();

    private void OnEnable()
    {
        Machine.OnFoodItemProduced += HandleFoodItemProduced;
    }

    private void OnDisable()
    {
        Machine.OnFoodItemProduced -= HandleFoodItemProduced;
    }

    private void HandleFoodItemProduced(
        FoodItemData foodItemData)
    {
        if (foodItemData == null)
            return;

        foreach (FoodCategory category in
                 System.Enum.GetValues(typeof(FoodCategory)))
        {
            if (foodItemData.HasCategory(category))
            {
                if (!categoryCounts.ContainsKey(category))
                {
                    categoryCounts[category] = 0;
                }

                categoryCounts[category]++;
            }
        }
    }

    public int GetCategoryCount(
        FoodCategory category)
    {
        if (categoryCounts.TryGetValue(
                category,
                out int count))
        {
            return count;
        }

        return 0;
    }
}
