using System.Collections.Generic;
using UnityEngine;

public class ProductionObjective : MonoBehaviour
{
    public event System.Action<ProductionObjective> OnCompleted;

    [SerializeField]
    private ProductionObjectiveData objectiveData;

    private Dictionary<FoodCategory, int> startingProductionCounts =
        new Dictionary<FoodCategory, int>();

    private Dictionary<FoodCategory, int> currentProgress =
        new Dictionary<FoodCategory, int>();

    private bool completionEventRaised;

    private ProductionObjectiveState state =
    ProductionObjectiveState.NotStarted;

    public ProductionObjectiveState State =>
        state;

    public ProductionObjectiveData ObjectiveData =>
        objectiveData;

    public bool IsCompleted
    {
        get
        {
            if (objectiveData == null ||
                objectiveData.Requirements == null ||
                objectiveData.Requirements.Length == 0)
            {
                return false;
            }

            foreach (ProductionObjectiveRequirement requirement
                     in objectiveData.Requirements)
            {
                if (!currentProgress.ContainsKey(requirement.Category))
                    return false;

                if (currentProgress[requirement.Category] <
                    requirement.RequiredQuantity)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public int GetCurrentProgress(
        FoodCategory category)
    {
        if (currentProgress.TryGetValue(
                category,
                out int progress))
        {
            return progress;
        }

        return 0;
    }

    public int GetRequiredQuantity(
        FoodCategory category)
    {
        if (objectiveData == null ||
            objectiveData.Requirements == null)
        {
            return 0;
        }

        foreach (ProductionObjectiveRequirement requirement
                 in objectiveData.Requirements)
        {
            if (requirement.Category == category)
                return requirement.RequiredQuantity;
        }

        return 0;
    }

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
        if (state == ProductionObjectiveState.Completed)
            return;

        if (objectiveData == null ||
            objectiveData.Requirements == null)
        {
            return;
        }

        bool isRequiredCategory = false;

        foreach (ProductionObjectiveRequirement requirement
                 in objectiveData.Requirements)
        {
            if (requirement.Category != category)
                continue;

            isRequiredCategory = true;

            int baseline =
                startingProductionCounts.ContainsKey(category)
                    ? startingProductionCounts[category]
                    : 0;

            int progress = Mathf.Clamp(
                count - baseline,
                0,
                requirement.RequiredQuantity
            );

            currentProgress[category] = progress;
        }

        if (!isRequiredCategory)
            return;

        if (IsCompleted && !completionEventRaised)
        {
            state = ProductionObjectiveState.Completed;

            completionEventRaised = true;
            OnCompleted?.Invoke(this);
        }
    }

    public void Initialize(
        ProductionObjectiveData data)
    {
        if (state == ProductionObjectiveState.Completed)
        {
            Debug.LogWarning(
                $"Cannot initialize completed objective " +
                $"'{name}'."
            );

            return;
        }

        objectiveData = data;

        startingProductionCounts.Clear();
        currentProgress.Clear();

        completionEventRaised = false;

        if (objectiveData == null ||
            objectiveData.Requirements == null)
        {
            return;
        }

        if (objectiveData == null ||
            objectiveData.Requirements == null ||
            objectiveData.Requirements.Length == 0)
        {
            Debug.LogError(
                "Cannot initialize ProductionObjective: " +
                "ObjectiveData is null or contains no requirements.",
                this
            );

            return;
        }

        foreach (ProductionObjectiveRequirement requirement
         in objectiveData.Requirements)
        {
            if (requirement == null)
            {
                Debug.LogError(
                    "Cannot initialize ProductionObjective: " +
                    "A requirement is null.",
                    this
                );

                return;
            }
        }

        state = ProductionObjectiveState.Active;

        ProductionTracker tracker =
            FindFirstObjectByType<ProductionTracker>();

        foreach (ProductionObjectiveRequirement requirement
                 in objectiveData.Requirements)
        {
            int startingCount = 0;

            if (tracker != null)
            {
                startingCount =
                    tracker.GetCategoryCount(
                        requirement.Category
                    );
            }

            startingProductionCounts[
                requirement.Category
            ] = startingCount;

            currentProgress[
                requirement.Category
            ] = 0;
        }
    }
}