using UnityEngine;

[CreateAssetMenu(
    fileName = "ProductionObjective_",
    menuName = "Objectives/Production Objective"
)]
public class ProductionObjectiveData : ScriptableObject
{
    [SerializeField] private FoodCategory targetCategory;

    [SerializeField] private int requiredQuantity = 1;

    public FoodCategory TargetCategory =>
        targetCategory;

    public int RequiredQuantity =>
        requiredQuantity;
}