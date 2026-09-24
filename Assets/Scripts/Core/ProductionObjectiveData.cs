using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "ProductionObjective_",
    menuName = "Objectives/Production Objective"
)]
public class ProductionObjectiveData : ScriptableObject
{
    [SerializeField]
    private ProductionObjectiveRequirement[] requirements;

    public ProductionObjectiveRequirement[] Requirements =>
        requirements;

    [Header("Display")]
    [SerializeField]
    private string title;

    [TextArea(2, 4)]
    [SerializeField]
    private string description;

    public string Title => title;
    public string Description => description;

    private void OnValidate()
    {
        if (requirements == null ||
            requirements.Length == 0)
        {
            Debug.LogError(
                $"Objective '{name}' is invalid. " +
                $"Reason: Objective must contain at least one requirement.",
                this
            );

            return;
        }

        HashSet<string> usedRequirements =
            new HashSet<string>();

        for (int i = 0; i < requirements.Length; i++)
        {
            if (requirements[i] == null)
            {
                Debug.LogError(
                    $"Objective '{name}' is invalid. " +
                    $"Reason: Requirement {i} is null.",
                    this
                );

                continue;
            }

            if (requirements[i].RequiredQuantity <= 0)
            {
                Debug.LogError(
                    $"Objective '{name}' is invalid. " +
                    $"Reason: Requirement {i} has an invalid " +
                    $"Required Quantity: " +
                    $"{requirements[i].RequiredQuantity}.",
                    this
                );
            }

            string requirementKey =
                requirements[i].Category.ToString() +
                "_" +
                requirements[i].Source.ToString();

            if (!usedRequirements.Add(requirementKey))
            {
                Debug.LogError(
                    $"Objective '{name}' is invalid. " +
                    $"Reason: Requirement {i} uses the same " +
                    $"FoodCategory and Source: " +
                    $"{requirements[i].Category} / " +
                    $"{requirements[i].Source}.",
                    this
                );
            }
        }
    }
}