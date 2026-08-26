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

        HashSet<FoodCategory> usedCategories =
            new HashSet<FoodCategory>();

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

            if (!usedCategories.Add(
                    requirements[i].Category))
            {
                Debug.LogError(
                    $"Objective '{name}' is invalid. " +
                    $"Reason: Requirement {i} uses the same " +
                    $"FoodCategory: {requirements[i].Category}.",
                    this
                );
            }
        }
    }
}