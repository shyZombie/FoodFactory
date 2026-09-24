using System.Collections.Generic;
using UnityEngine;

public class ProductionObjective : MonoBehaviour
{
    public event System.Action<ProductionObjective> OnCompleted;
    public event System.Action<ProductionObjective> OnProgressChanged;

    [SerializeField]
    private ProductionObjectiveData objectiveData;

    private Dictionary<string, int> startingProductionCounts =
        new Dictionary<string, int>();

    private Dictionary<string, int> startingDeliveryCounts =
        new Dictionary<string, int>();

    private Dictionary<string, int> currentProgress =
        new Dictionary<string, int>();

    private bool completionEventRaised;

    private ProductionObjectiveState state =
    ProductionObjectiveState.NotStarted;

    public ProductionObjectiveState State =>
        state;

    public ProductionObjectiveData ObjectiveData =>
        objectiveData;

    private string GetRequirementKey(
    FoodCategory category,
    ObjectiveRequirementSource source)
    {
        return category + "_" + source;
    }

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
                string requirementKey =
                    GetRequirementKey(
                        requirement.Category,
                        requirement.Source
                    );

                if (!currentProgress.ContainsKey(requirementKey))
                    return false;

                if (currentProgress[requirementKey] <
                    requirement.RequiredQuantity)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public int GetCurrentProgress(
        FoodCategory category,
        ObjectiveRequirementSource source)
    {
        string requirementKey =
            GetRequirementKey(
                category,
                source
            );

        if (currentProgress.TryGetValue(
                requirementKey,
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
    public List<ProductionObjectiveProgress>
    GetProgressSnapshot()
    {
        List<ProductionObjectiveProgress> progress =
            new List<ProductionObjectiveProgress>();

        if (objectiveData == null ||
            objectiveData.Requirements == null)
        {
            return progress;
        }

        foreach (ProductionObjectiveRequirement requirement
                 in objectiveData.Requirements)
        {
            int current =
                GetCurrentProgress(
                    requirement.Category,
                    requirement.Source
                );

            progress.Add(
                new ProductionObjectiveProgress(
                    requirement.Category,
                    requirement.Source,
                    current,
                    requirement.RequiredQuantity
                )
            );
        }

        return progress;
    }

    private void OnEnable()
    {
        ProductionTracker.OnCategoryCountChanged +=
            HandleProductionCategoryCountChanged;

        DeliveryTracker.OnCategoryCountChanged +=
            HandleDeliveryCategoryCountChanged;
    }

    private void OnDisable()
    {
        ProductionTracker.OnCategoryCountChanged -=
            HandleProductionCategoryCountChanged;

        DeliveryTracker.OnCategoryCountChanged -=
            HandleDeliveryCategoryCountChanged;
    }
    private void HandleProductionCategoryCountChanged(
        FoodCategory category,
        int count)
    {
        HandleCategoryCountChanged(
            ObjectiveRequirementSource.Production,
            category,
            count
        );
    }

    private void HandleDeliveryCategoryCountChanged(
        FoodCategory category,
        int count)
    {
        HandleCategoryCountChanged(
            ObjectiveRequirementSource.Delivery,
            category,
            count
        );
    }

    private void HandleCategoryCountChanged(
        ObjectiveRequirementSource source,
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
            if (requirement.Source != source)
                continue;

            string requirementKey =
                GetRequirementKey(
                    category,
                    source
                );

            isRequiredCategory = true;

            int baseline = 0;

            if (source == ObjectiveRequirementSource.Production)
            {
                baseline =
                    startingProductionCounts.ContainsKey(requirementKey)
                        ? startingProductionCounts[requirementKey]
                        : 0;
            }
            else if (source == ObjectiveRequirementSource.Delivery)
            {
                baseline =
                    startingDeliveryCounts.ContainsKey(requirementKey)
                        ? startingDeliveryCounts[requirementKey]
                        : 0;
            }

            int progress = Mathf.Clamp(
                count - baseline,
                0,
                requirement.RequiredQuantity
            );

            int previousProgress =
                currentProgress.ContainsKey(requirementKey)
                    ? currentProgress[requirementKey]
                    : 0;

            currentProgress[requirementKey] = progress;

            if (currentProgress[requirementKey] != previousProgress)
            {
                OnProgressChanged?.Invoke(this);
            }
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

        ProductionTracker productionTracker =
            FindFirstObjectByType<ProductionTracker>();

        DeliveryTracker deliveryTracker =
            FindFirstObjectByType<DeliveryTracker>();

        foreach (ProductionObjectiveRequirement requirement
                 in objectiveData.Requirements)
        {
            int startingCount = 0;
            string requirementKey =
                GetRequirementKey(
                    requirement.Category,
                    requirement.Source
                );

            if (requirement.Source ==
                ObjectiveRequirementSource.Production)
            {
                if (productionTracker != null)
                {
                    startingCount =
                        productionTracker.GetCategoryCount(
                            requirement.Category
                        );
                }

                startingProductionCounts[
                    requirementKey
                ] = startingCount;
            }
            else if (requirement.Source ==
                     ObjectiveRequirementSource.Delivery)
            {
                if (deliveryTracker != null)
                {
                    startingCount =
                        deliveryTracker.GetDeliveredCount(
                            requirement.Category
                        );
                }

                startingDeliveryCounts[
                    requirementKey
                ] = startingCount;
            }

            currentProgress[
                requirementKey
            ] = 0;
        }
    }

 }    