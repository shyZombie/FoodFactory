using System;
using UnityEngine;

[Serializable]
public class ProductionObjectiveRequirement
{
    [SerializeField]
    private FoodCategory category;

    [SerializeField]
    private ObjectiveRequirementSource source =
        ObjectiveRequirementSource.Production;

    [SerializeField]
    private int requiredQuantity = 1;

    public FoodCategory Category =>
        category;
    public ObjectiveRequirementSource Source =>
        source;

    public int RequiredQuantity =>
        requiredQuantity;

}
