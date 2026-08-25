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
}