using UnityEngine;

public class ProductionObjective : MonoBehaviour
{
    [SerializeField] private FoodCategory targetCategory;
    [SerializeField] private int requiredQuantity = 1;

    private int currentProgress;

    public FoodCategory TargetCategory => targetCategory;
    public int RequiredQuantity => requiredQuantity;
    public int CurrentProgress => currentProgress;

    public bool IsCompleted =>
        currentProgress >= requiredQuantity;

    private void OnEnable()
    {
        ProductionTracker.OnCategoryCountChanged +=
            HandleCategoryCountChanged;
    }

    private void OnDisable()
    {
        ProductionTracker.OnCategoryCountChanged -=
            HandleCategoryCountChanged;
    }

    private void HandleCategoryCountChanged(
        FoodCategory category,
        int count)
    {

        if (category != targetCategory)
            return;

        currentProgress = Mathf.Min(
            count,
            requiredQuantity
        );
    }
}
