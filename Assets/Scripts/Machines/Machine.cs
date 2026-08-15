using UnityEngine;

public class Machine : GridObject
{
    [SerializeField] protected float processingTime = 1f;

    protected GridManager gridManager;

    public void Initialize(GridManager manager)
    {
        gridManager = manager;
    }

    public virtual bool CanProcess(FoodItem foodItem)
    {
        return true;
    }

    public virtual void Process(FoodItem foodItem)
    {
        Debug.Log(
            $"Machine processing {foodItem.ItemData.ItemName}"
        );
    }
}
