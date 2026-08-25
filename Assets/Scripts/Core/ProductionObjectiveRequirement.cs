using System;
using UnityEngine;

[Serializable]
public class ProductionObjectiveRequirement
{
    [SerializeField]
    private FoodCategory category;

    [SerializeField]
    private int requiredQuantity = 1;

    public FoodCategory Category =>
        category;

    public int RequiredQuantity =>
        requiredQuantity;

}
