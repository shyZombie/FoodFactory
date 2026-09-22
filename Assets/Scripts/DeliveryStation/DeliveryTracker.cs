using System.Collections.Generic;
using UnityEngine;

public class DeliveryTracker : MonoBehaviour
{
    private Dictionary<FoodCategory, int> deliveredByCategory =
        new Dictionary<FoodCategory, int>();
    public static event System.Action<FoodCategory, int>
        OnCategoryCountChanged;

    private void Awake()
    {
        LoadDeliveryData();
    }

    private void LoadDeliveryData()
    {
        DeliveryTrackerSaveData saveData =
            DeliveryTrackerSaveSystem.Load();

        if (saveData == null)
        {
            return;
        }

        deliveredByCategory.Clear();

        if (saveData.categoryCounts == null)
        {
            return;
        }

        foreach (DeliveryCategorySaveData entry
                 in saveData.categoryCounts)
        {
            if (entry.count <= 0)
            {
                continue;
            }

            deliveredByCategory[entry.category] = entry.count;
        }
    }

    public int GetDeliveredCount(FoodCategory category)
    {
        if (deliveredByCategory.TryGetValue(category, out int count))
            return count;

        return 0;
    }

    public DeliveryTrackerSaveData CreateSaveData()
    {
        DeliveryTrackerSaveData saveData =
            new DeliveryTrackerSaveData();

        foreach (KeyValuePair<FoodCategory, int> entry
                 in deliveredByCategory)
        {
            saveData.categoryCounts.Add(
                new DeliveryCategorySaveData
                {
                    category = entry.Key,
                    count = entry.Value
                }
            );
        }

        return saveData;
    }
    private void SaveDeliveryData()
    {
        DeliveryTrackerSaveData saveData =
            CreateSaveData();

        DeliveryTrackerSaveSystem.Save(
            saveData
        );
    }

    public void RegisterDelivery(FoodCategory category, int quantity = 1)
    {
        if (quantity <= 0)
            return;

        if (!deliveredByCategory.ContainsKey(category))
            deliveredByCategory[category] = 0;

        deliveredByCategory[category] += quantity;

        OnCategoryCountChanged?.Invoke(
            category,
            deliveredByCategory[category]
        );

        Debug.Log(
            $"DeliveryTracker: delivered {quantity} item(s) of category " +
            $"{category}. Total: {deliveredByCategory[category]}."
        );

        SaveDeliveryData();
    }
}
