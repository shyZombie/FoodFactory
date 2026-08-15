using UnityEngine;

public class Cutter : Machine
{
    //[SerializeField] private Vector2 outputDirection = Vector2.right;

    public override bool CanProcess(FoodItem foodItem)
    {
        if (isProcessing)
            return false;

        return base.CanProcess(foodItem);
    }
}