using UnityEngine;

public class ProductionObjective : MonoBehaviour
{
    public event System.Action<ProductionObjective> OnCompleted;
    [SerializeField]
    private ProductionObjectiveData objectiveData;

    private int currentProgress;
    private int startingProductionCount;
    private bool completionEventRaised;

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

        currentProgress = Mathf.Clamp(
            count - startingProductionCount,
            0,
            RequiredQuantity
        );

        if (IsCompleted && !completionEventRaised)
        {
            completionEventRaised = true;
            OnCompleted?.Invoke(this);
        }
    }

    public void Initialize(
        ProductionObjectiveData data)
    {
        objectiveData = data;

        currentProgress = 0;
        startingProductionCount = 0;

        ProductionTracker tracker =
            FindFirstObjectByType<ProductionTracker>();

        if (tracker != null)
        {
            startingProductionCount =
                tracker.GetCategoryCount(
                    objectiveData.TargetCategory
                );
        }
        Debug.Log(
            $"Objective initialized: {objectiveData.TargetCategory} " +
            $"| Starting Production Count: {startingProductionCount}"
        );
    }
}
