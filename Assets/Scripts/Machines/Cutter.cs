using UnityEngine;

public class Cutter : Machine
{
    [SerializeField] private FoodItemData inputItem;
    [SerializeField] private FoodItemData outputItem;

    public override bool CanProcess(FoodItem foodItem)
    {
        return foodItem.ItemData == inputItem;
    }

    public override void Process(FoodItem foodItem)
    {
        Debug.Log(
            $"Cutter processed {inputItem.ItemName} " +
            $"into {outputItem.ItemName}"
        );
    }
}
