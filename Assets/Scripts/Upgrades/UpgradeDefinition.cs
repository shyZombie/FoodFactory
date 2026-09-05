using UnityEngine;

[CreateAssetMenu(
    fileName = "Upgrade_",
    menuName = "Food Factory/Upgrade Definition"
)]
public class UpgradeDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField]
    private string upgradeId;

    [SerializeField]
    private UpgradeTarget target;

    [SerializeField]
    private int tier;

    [Header("Requirement")]
    [SerializeField]
    private FoodCategory requiredCategory;

    [SerializeField]
    private int requiredRecipeCount;

    [Header("Effect")]
    [SerializeField]
    private float speedMultiplier = 1.5f;

    public string UpgradeId => upgradeId;

    public UpgradeTarget Target => target;

    public int Tier => tier;

    public FoodCategory RequiredCategory =>
        requiredCategory;

    public int RequiredRecipeCount =>
        requiredRecipeCount;

    public float SpeedMultiplier =>
        speedMultiplier;
}
