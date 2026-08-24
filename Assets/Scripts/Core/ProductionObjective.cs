using UnityEngine;

public class ProductionObjective : MonoBehaviour
{
    [SerializeField]
    private ProductionObjectiveData objectiveData;

    private int currentProgress;

    public FoodCategory TargetCategory =>
        objectiveData != null
            ? objectiveData.TargetCategory
            : default;

    public int RequiredQuantity =>
        objectiveData != null
            ? objectiveData.RequiredQuantity
            : 0;
    public int CurrentProgress => currentProgress;

    public bool IsCompleted =>
        currentProgress >= RequiredQuantity;

    private void OnEnable()
    {
        ProductionTracker.OnCategoryCountChanged +=
            HandleCategoryCountChanged;

        if (objectiveData == null)
            return;

        ProductionTracker tracker =
            FindFirstObjectByType<ProductionTracker>();

        if (tracker != null)
        {
            currentProgress = Mathf.Min(
                tracker.GetCategoryCount(
                    objectiveData.TargetCategory
                ),
                objectiveData.RequiredQuantity
            );
        }
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

        if (objectiveData == null)
            return;

        if (category != objectiveData.TargetCategory)
            return;

        currentProgress = Mathf.Min(
            count,
            RequiredQuantity
        );
    }
}
