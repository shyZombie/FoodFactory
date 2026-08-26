using System.Collections.Generic;
using UnityEngine;

public class ProductionObjectiveUI : MonoBehaviour
{
    [SerializeField]
    private Transform requirementsContainer;

    [SerializeField]
    private ProductionObjectiveRequirementUI requirementPrefab;
    private ProductionObjective objective;

    public void Bind(ProductionObjective objective)
    {
        Unbind();

        if (objective == null)
        {
            Debug.LogWarning(
                "ProductionObjectiveUI: " +
                "Cannot bind to a null objective.",
                this
            );

            return;
        }

        this.objective = objective;

        this.objective.OnProgressChanged +=
            HandleProgressChanged;

        this.objective.OnCompleted +=
            HandleObjectiveCompleted;

        Refresh();
    }

    private void Unbind()
    {
        if (objective == null)
            return;

        objective.OnProgressChanged -=
            HandleProgressChanged;

        objective.OnCompleted -=
            HandleObjectiveCompleted;

        objective = null;
    }

    private void HandleProgressChanged(
        ProductionObjective objective)
    {
        Refresh();
    }

    private void HandleObjectiveCompleted(
        ProductionObjective objective)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (objective == null)
            return;

        ClearRequirements();

        List<ProductionObjectiveProgress> progress =
            objective.GetProgressSnapshot();

        foreach (ProductionObjectiveProgress item
                 in progress)
        {
            ProductionObjectiveRequirementUI requirementUI =
                Instantiate(
                    requirementPrefab,
                    requirementsContainer
                );

            requirementUI.SetProgress(item);
        }
    }

    private void ClearRequirements()
    {
        if (requirementsContainer == null)
            return;

        for (int i = requirementsContainer.childCount - 1;
             i >= 0;
             i--)
        {
            Destroy(
                requirementsContainer.GetChild(i).gameObject
            );
        }
    }

    private void OnDisable()
    {
        Unbind();
    }
}
