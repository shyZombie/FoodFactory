using System.Collections.Generic;
using UnityEngine;

public class ProductionObjectiveUI : MonoBehaviour
{
    [SerializeField]
    private Transform requirementsContainer;

    [SerializeField]
    private ProductionObjectiveRequirementUI requirementPrefab;
    private ProductionObjective objective;
    private List<ProductionObjectiveRequirementUI>
    requirementUIRows =
        new List<ProductionObjectiveRequirementUI>();

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
        if (objective != null)
        {
            objective.OnProgressChanged -=
                HandleProgressChanged;

            objective.OnCompleted -=
                HandleObjectiveCompleted;

            objective = null;
        }

        ClearRequirementRows();
    }
    private void ClearRequirementRows()
    {
        for (int i = requirementUIRows.Count - 1;
             i >= 0;
             i--)
        {
            if (requirementUIRows[i] != null)
            {
                Destroy(
                    requirementUIRows[i].gameObject
                );
            }
        }

        requirementUIRows.Clear();
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

        List<ProductionObjectiveProgress> progress =
            objective.GetProgressSnapshot();

        EnsureRequirementRows(progress.Count);

        for (int i = 0; i < progress.Count; i++)
        {
            requirementUIRows[i].SetProgress(
                progress[i]
            );
        }
    }

    private void EnsureRequirementRows(int requiredCount)
    {
        while (requirementUIRows.Count < requiredCount)
        {
            ProductionObjectiveRequirementUI requirementUI =
                Instantiate(
                    requirementPrefab,
                    requirementsContainer
                );

            requirementUIRows.Add(requirementUI);
        }

        while (requirementUIRows.Count > requiredCount)
        {
            int lastIndex =
                requirementUIRows.Count - 1;

            ProductionObjectiveRequirementUI requirementUI =
                requirementUIRows[lastIndex];

            requirementUIRows.RemoveAt(lastIndex);

            Destroy(requirementUI.gameObject);
        }
    }

    private void OnDisable()
    {
        Unbind();
    }
}
