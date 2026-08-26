using System;

[Serializable]
public class ProductionObjectiveProgress
{
    public FoodCategory Category { get; }
    public int Current { get; }
    public int Required { get; }

    public bool IsCompleted =>
        Current >= Required;

    public ProductionObjectiveProgress(
        FoodCategory category,
        int current,
        int required)
    {
        Category = category;
        Current = current;
        Required = required;
    }
}
